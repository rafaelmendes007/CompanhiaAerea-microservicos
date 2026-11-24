using Vendas.Api.DTOs;
using Vendas.Api.Models;

namespace Vendas.Api.Services
{
    public enum TipoResultadoVenda
    {
        Confirmada,
        PagamentoRecusado,
        PassageiroNaoEncontrado,
        VooNaoEncontrado,
        VooLotado,
        DadosInvalidos,
        ServicoIndisponivel
    }

    public record ResultadoVenda(TipoResultadoVenda Tipo, Venda? Venda = null, string? Mensagem = null);
    
    public interface IVendaService
    {
        Task<ResultadoVenda> CriarAsync(CriarVendaDto dto);
        Task<Venda?> ObterPorIdAsync(int id);
        Task<List<Venda>> ListarAsync(long? passageiroId);
    }
}
