using Microsoft.EntityFrameworkCore;
using Voos.Api.Data;
using Voos.Api.Models;

namespace Voos.Api.Repositories
{ 

    public class VooRepository : IVooRepository
    {
        private readonly VoosDbContext _context;

        public VooRepository(VoosDbContext context)
        {
            _context = context;
        }

        public async Task<List<Voo>> ListarAsync(string? origem, string? destino, DateTime? dia)
        {
            var query = _context.Voos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(origem))
                query = query.Where(v => v.Origem == origem);

            if (!string.IsNullOrWhiteSpace(destino))
                query = query.Where(v => v.Destino == destino);

            if (dia.HasValue)
            {
                var inicio = dia.Value.Date;
                var fim = inicio.AddDays(1);
                query = query.Where(v => v.DataPartida >= inicio && v.DataPartida < fim);
            }

            return await query.OrderBy(v => v.DataPartida).ToListAsync();
        }

        public async Task<Voo?> ObterPorIdAsync(int id)
        {
            return await _context.Voos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Voo> AdicionarAsync(Voo voo)
        {
            _context.Voos.Add(voo);
            await _context.SaveChangesAsync();
            return voo;
        }

        // UPDATE direto no banco: só diminui se ainda houver assento.
        // Assim, duas compras ao mesmo tempo nunca deixam o número negativo.
        public async Task<bool> DecrementarAssentoAsync(int id)
        {
            var linhas = await _context.Voos
                .Where(v => v.Id == id && v.AssentosDisponiveis > 0)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.AssentosDisponiveis, v => v.AssentosDisponiveis - 1));
            return linhas == 1;
        }

        public async Task<bool> IncrementarAssentoAsync(int id)
        {
            var linhas = await _context.Voos
                .Where(v => v.Id == id && v.AssentosDisponiveis < v.TotalAssentos)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.AssentosDisponiveis, v => v.AssentosDisponiveis + 1));
            return linhas == 1;
        }
    }
}