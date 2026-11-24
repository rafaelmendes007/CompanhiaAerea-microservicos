using Vendas.Api.DTOs;

namespace Vendas.Api.Clients
{
    public enum ResultadoReserva { Sucesso, Lotado, VooNaoEncontrado }

    public interface IVoosClient
    {
        Task<VooDto?> ObterPorIdAsync(int id);
        Task<ResultadoReserva> ReservarAssentoAsync(int id);
        Task LiberarAssentoAsync(int id);
    }
}
