using Voos.Api.DTOs;
using Voos.Api.Models;

namespace Voos.Api.Services
{

    public enum ResultadoOperacao { Sucesso, NaoEncontrado, Conflito}

    public interface IVooService
    {
        Task<List<Voo>> ListarAsync(string? origem, string? destino, DateOnly? data);
        Task<Voo?> ObterPorIdAsync(int id);
        Task<Voo> CriarAsync(CriarVooDto dto);
        Task<ResultadoOperacao> ReservarAssentoAsync(int id);
        Task<ResultadoOperacao> LiberarAssentoAsync(int id);
    }
}
