using Microsoft.EntityFrameworkCore;
using Vendas.Api.Models;

namespace Vendas.Api.Data
{
    public class VendasDbContext : DbContext
    {
        public VendasDbContext(DbContextOptions<VendasDbContext> options) : base(options) { }

        public DbSet<Venda> Vendas => Set<Venda>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Venda>(e =>
            {
                e.ToTable("vendas");
                e.Property(v => v.Valor).HasPrecision(10, 2);
                e.Property(v => v.MetodoPagamento).HasMaxLength(10).IsRequired();
                e.Property(v => v.Status).HasMaxLength(15).IsRequired();
                e.Property(v => v.DataCompra).HasColumnType("timestamp without time zone");
                e.HasIndex(v => v.PassageiroId); // acelera a busca "minhas compras"
            });
        }
    }
}
