using Vendas.Api.DTOs;

namespace Vendas.Api.Clients
{
    public interface IPassageirosClient
    {
        Task<PassageiroDto?> ObterPorIdAsync(long id); // null = não existe
    }
}
