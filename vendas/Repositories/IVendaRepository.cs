using Vendas.Api.Models;

namespace Vendas.Api.Repositories
{
    public interface IVendaRepository
    {
        Task<Venda> AdicionarAsync(Venda venda);
        Task AtualizarAsync(Venda venda);
        Task<Venda?> ObterPorIdAsync(int id);
        Task<List<Venda>> ListarAsync(long? passageiroId);
    }
}
