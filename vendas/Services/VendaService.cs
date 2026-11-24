using Vendas.Api.Clients;
using Vendas.Api.DTOs;
using Vendas.Api.Models;
using Vendas.Api.Repositories;

namespace Vendas.Api.Services
{
    public class VendaService : IVendaService
    {
        private static readonly string[] MetodosAceitos = { "CARTAO", "PIX" };

        private readonly IVendaRepository _repository;
        private readonly IPassageirosClient _passageiros;
        private readonly IVoosClient _voos;
        private readonly IPagamentosClient _pagamentos;

        public VendaService(IVendaRepository repository, IPassageirosClient passageiros,
                            IVoosClient voos, IPagamentosClient pagamentos)
        {
            _repository = repository;
            _passageiros = passageiros;
            _voos = voos;
            _pagamentos = pagamentos;
        }

        public async Task<ResultadoVenda> CriarAsync(CriarVendaDto dto)
        {
            var metodo = dto.MetodoPagamento.Trim().ToUpper();
            if (!MetodosAceitos.Contains(metodo))
                return new ResultadoVenda(TipoResultadoVenda.DadosInvalidos,
                    Mensagem: "O método de pagamento deve ser CARTAO ou PIX.");

            Venda? venda = null;
            var assentoReservado = false;

            try
            {
                // 1. O passageiro existe?
                var passageiro = await _passageiros.ObterPorIdAsync(dto.PassageiroId);
                if (passageiro is null)
                    return new ResultadoVenda(TipoResultadoVenda.PassageiroNaoEncontrado,
                        Mensagem: "Passageiro não encontrado.");

                // 2. O voo existe? Qual o preço?
                var voo = await _voos.ObterPorIdAsync(dto.VooId);
                if (voo is null)
                    return new ResultadoVenda(TipoResultadoVenda.VooNaoEncontrado,
                        Mensagem: "Voo não encontrado.");

                // 3. Registra a venda como PENDENTE (já ganha um Id)
                venda = new Venda
                {
                    PassageiroId = dto.PassageiroId,
                    VooId = dto.VooId,
                    Valor = voo.Preco,
                    MetodoPagamento = metodo,
                    Status = StatusVenda.Pendente,
                    DataCompra = HorarioDeFortaleza()
                };
                await _repository.AdicionarAsync(venda);

                // 4. Reserva o assento no serviço de Voos
                var reserva = await _voos.ReservarAssentoAsync(dto.VooId);
                if (reserva != ResultadoReserva.Sucesso)
                {
                    await MarcarComoRecusadaAsync(venda);
                    return reserva == ResultadoReserva.Lotado
                        ? new ResultadoVenda(TipoResultadoVenda.VooLotado, venda, "Não há assentos disponíveis neste voo.")
                        : new ResultadoVenda(TipoResultadoVenda.VooNaoEncontrado, venda, "Voo não encontrado.");
                }
                assentoReservado = true;

                // 5. Cobra no serviço de Pagamentos
                var pagamento = await _pagamentos.ProcessarAsync(
                    new PagamentoRequestDto(venda.Id, venda.PassageiroId, venda.Valor, metodo));
                venda.PagamentoId = pagamento.Id;

                // 6a. Aprovado: confirma a venda
                if (pagamento.Status.Equals("APROVADO", StringComparison.OrdinalIgnoreCase))
                {
                    venda.Status = StatusVenda.Confirmada;
                    await _repository.AtualizarAsync(venda);
                    return new ResultadoVenda(TipoResultadoVenda.Confirmada, venda);
                }

                // 6b. Recusado: devolve o assento e recusa a venda
                await _voos.LiberarAssentoAsync(dto.VooId);
                await MarcarComoRecusadaAsync(venda);
                return new ResultadoVenda(TipoResultadoVenda.PagamentoRecusado, venda, "Pagamento recusado.");
            }
            catch (ServicoIndisponivelException ex)
            {
                // Algum serviço caiu no meio do caminho: desfaz o que já foi feito
                if (assentoReservado)
                    await _voos.LiberarAssentoAsync(dto.VooId);
                if (venda is not null && venda.Id > 0)
                    await MarcarComoRecusadaAsync(venda);

                return new ResultadoVenda(TipoResultadoVenda.ServicoIndisponivel, venda, ex.Message);
            }
        }

        public Task<Venda?> ObterPorIdAsync(int id) => _repository.ObterPorIdAsync(id);

        public Task<List<Venda>> ListarAsync(long? passageiroId) => _repository.ListarAsync(passageiroId);

        private async Task MarcarComoRecusadaAsync(Venda venda)
        {
            venda.Status = StatusVenda.Recusada;
            await _repository.AtualizarAsync(venda);
        }

        // O servidor na nuvem roda em UTC; gravamos no horário de Fortaleza
        private static DateTime HorarioDeFortaleza()
        {
            var agora = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "America/Fortaleza");
            return DateTime.SpecifyKind(agora, DateTimeKind.Unspecified);
        }
    }
}
