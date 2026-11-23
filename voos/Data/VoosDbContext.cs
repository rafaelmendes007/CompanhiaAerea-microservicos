using Microsoft.EntityFrameworkCore;
using Voos.Api.Models;

namespace Voos.Api.Data
{
    // Ponte entre o código e o banco de dados
    public class VoosDbContext : DbContext
    {
        public VoosDbContext(DbContextOptions<VoosDbContext> options) : base(options) { }

        public DbSet<Voo> Voos => Set<Voo>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Voo>(e =>
            {
                e.ToTable("voos");
                e.Property(v => v.Codigo).HasMaxLength(10).IsRequired();
                e.Property(v => v.Origem).HasMaxLength(3).IsRequired();
                e.Property(v => v.Destino).HasMaxLength(3).IsRequired();
                e.Property(v => v.Preco).HasPrecision(10, 2);
                // horário local do voo, sem fuso
                e.Property(v => v.DataPartida).HasColumnType("timestamp without time zone");
                e.Property(v => v.DataChegada).HasColumnType("timestamp without time zone");
            });
        }
    }
}   
