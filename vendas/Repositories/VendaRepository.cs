using Microsoft.EntityFrameworkCore;
using Vendas.Api.Data;
using Vendas.Api.Models;

namespace Vendas.Api.Repositories
{
    public class VendaRepository: IVendaRepository
    {
        private readonly VendasDbContext _context;

        public VendaRepository(VendasDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> AdicionarAsync(Venda venda)
        {
            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();
            return venda;
        }

        public async Task AtualizarAsync(Venda venda)
        {
            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();
        }

        public async Task<Venda?> ObterPorIdAsync(int id)
        {
            return await _context.Vendas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Venda>> ListarAsync(long? passageiroId)
        {
            var query = _context.Vendas.AsNoTracking().AsQueryable();

            if (passageiroId.HasValue)
                query = query.Where(v => v.PassageiroId == passageiroId.Value);

            return await query.OrderByDescending(v => v.DataCompra).ToListAsync();
        }
    }
}
