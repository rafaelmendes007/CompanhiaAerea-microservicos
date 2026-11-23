using Voos.Api.Models;

namespace Voos.Api.Repositories
{
    // O "contrato" do que o repositório sabe fazer
    public interface IVooRepository
    {
        Task<List<Voo>> ListarAsync(string? origem, string? destino, DateTime? dia);
        Task<Voo?> ObterPorIdAsync(int id);
        Task<Voo> AdicionarAsync(Voo voo);
        Task<bool> DecrementarAssentoAsync(int id);
        Task<bool> IncrementarAssentoAsync(int id);
    }
}
