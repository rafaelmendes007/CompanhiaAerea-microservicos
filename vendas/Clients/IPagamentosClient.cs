using Vendas.Api.DTOs;

namespace Vendas.Api.Clients
{
    public interface IPagamentosClient
    {
        Task<PagamentoResponseDto> ProcessarAsync(PagamentoRequestDto pedido);
    }
}
