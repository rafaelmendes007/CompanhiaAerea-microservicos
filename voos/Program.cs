using Microsoft.EntityFrameworkCore;
using Voos.Api.Data;
using Voos.Api.Models;
using Voos.Api.Repositories;
using Voos.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Banco de dados
builder.Services.AddDbContext<VoosDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VoosDb")));

// Injeção de dependência: "quando alguém pedir a interface, entregue esta classe"
builder.Services.AddScoped<IVooRepository, VooRepository>();
builder.Services.AddScoped<IVooService, VooService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Permite que o frontend React (outro endereço) chame esta API pelo navegador
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Cria a tabela se não existir e insere alguns voos de exemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VoosDbContext>();
    db.Database.EnsureCreated();

    if (!db.Voos.Any())
    {
        db.Voos.AddRange(
            new Voo
            {
                Codigo = "AE1001",
                Origem = "FOR",
                Destino = "GRU",
                DataPartida = new DateTime(2026, 12, 15, 8, 0, 0),
                DataChegada = new DateTime(2026, 12, 15, 11, 30, 0),
                Preco = 899.90m,
                TotalAssentos = 120,
                AssentosDisponiveis = 120
            },
            new Voo
            {
                Codigo = "AE1002",
                Origem = "GRU",
                Destino = "FOR",
                DataPartida = new DateTime(2026, 12, 20, 14, 0, 0),
                DataChegada = new DateTime(2026, 12, 20, 17, 20, 0),
                Preco = 799.90m,
                TotalAssentos = 120,
                AssentosDisponiveis = 120
            },
            new Voo
            {
                Codigo = "AE2001",
                Origem = "FOR",
                Destino = "REC",
                DataPartida = new DateTime(2026, 12, 16, 9, 30, 0),
                DataChegada = new DateTime(2026, 12, 16, 10, 45, 0),
                Preco = 349.90m,
                TotalAssentos = 80,
                AssentosDisponiveis = 80
            }
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // documentação em /openapi/v1.json
}

app.UseCors();
app.MapControllers();

app.Run();