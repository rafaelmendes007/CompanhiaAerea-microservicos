using Voos.Api.DTOs;
using Voos.Api.Models;
using Voos.Api.Repositories;

namespace Voos.Api.Services
{
    public class VooService : IVooService
    {
        private readonly IVooRepository _repository;

        public VooService(IVooRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Voo>> ListarAsync(string? origem, string? destino, DateOnly? data)
        {
            DateTime? dia = data?.ToDateTime(TimeOnly.MinValue);
            return _repository.ListarAsync(origem?.Trim().ToUpper(), destino?.Trim().ToUpper(), dia);
        }

        public Task<Voo?> ObterPorIdAsync(int id) => _repository.ObterPorIdAsync(id);

        public async Task<Voo> CriarAsync(CriarVooDto dto)
        {
            var origem = dto.Origem.Trim().ToUpper();
            var destino = dto.Destino.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(dto.Codigo))
                throw new ArgumentException("O código do voo é obrigatório.");
            if (origem.Length != 3 || destino.Length != 3)
                throw new ArgumentException("Origem e destino devem ter 3 letras (ex.: FOR, GRU).");
            if (origem == destino)
                throw new ArgumentException("Origem e destino não podem ser iguais.");
            if (dto.DataChegada <= dto.DataPartida)
                throw new ArgumentException("A chegada deve ser depois da partida.");
            if (dto.Preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.");
            if (dto.TotalAssentos <= 0)
                throw new ArgumentException("O total de assentos deve ser maior que zero.");

            var voo = new Voo
            {
                Codigo = dto.Codigo.Trim().ToUpper(),
                Origem = origem,
                Destino = destino,
                DataPartida = DateTime.SpecifyKind(dto.DataPartida, DateTimeKind.Unspecified),
                DataChegada = DateTime.SpecifyKind(dto.DataChegada, DateTimeKind.Unspecified),
                Preco = dto.Preco,
                TotalAssentos = dto.TotalAssentos,
                AssentosDisponiveis = dto.TotalAssentos // voo novo começa com todos livres
            };

            return await _repository.AdicionarAsync(voo);
        }

        public async Task<ResultadoOperacao> ReservarAssentoAsync(int id)
        {
            if (await _repository.ObterPorIdAsync(id) is null)
                return ResultadoOperacao.NaoEncontrado;

            return await _repository.DecrementarAssentoAsync(id)
                ? ResultadoOperacao.Sucesso
                : ResultadoOperacao.Conflito; // sem assentos
        }

        public async Task<ResultadoOperacao> LiberarAssentoAsync(int id)
        {
            if (await _repository.ObterPorIdAsync(id) is null)
                return ResultadoOperacao.NaoEncontrado;

            return await _repository.IncrementarAssentoAsync(id)
                ? ResultadoOperacao.Sucesso
                : ResultadoOperacao.Conflito; // já estava tudo livre
        }
    }
}
