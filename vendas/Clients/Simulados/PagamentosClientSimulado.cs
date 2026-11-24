using Vendas.Api.DTOs;

// Usado enquanto o serviço real de Pagamentos não estiver no ar.
// Mesma regra do contrato: acima de 5000.00 é recusado.
namespace Vendas.Api.Clients.Simulados
{
    public class PagamentosClientSimulado : IPagamentosClient
    {
        public Task<PagamentoResponseDto> ProcessarAsync(PagamentoRequestDto pedido)
        {
            var status = pedido.Valor > 5000m ? "RECUSADO" : "APROVADO";
            var resposta = new PagamentoResponseDto(Random.Shared.Next(1, 100000), status);
            return Task.FromResult(resposta);
        }
    }
}
