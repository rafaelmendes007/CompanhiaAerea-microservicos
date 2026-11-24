using Microsoft.EntityFrameworkCore;
using Vendas.Api.Clients;
using Vendas.Api.Clients.Simulados;
using Vendas.Api.Data;
using Vendas.Api.Repositories;
using Vendas.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Banco de dados (próprio do Vendas)
builder.Services.AddDbContext<VendasDbContext>(options =>
    options.UseNpgsql(config.GetConnectionString("VendasDb")));

// Camadas
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IVendaService, VendaService>();

// Clients HTTP para os outros serviços (endereços vêm da configuração)
builder.Services.AddHttpClient<IVoosClient, VoosClient>(client =>
{
    client.BaseAddress = new Uri(config["Servicos:VoosUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(10);
});

if (config.GetValue<bool>("Simulacao:Passageiros"))
{
    builder.Services.AddScoped<IPassageirosClient, PassageirosClientSimulado>();
}
else
{
    builder.Services.AddHttpClient<IPassageirosClient, PassageirosClient>(client =>
    {
        client.BaseAddress = new Uri(config["Servicos:PassageirosUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(10);
    });
}

if (config.GetValue<bool>("Simulacao:Pagamentos"))
{
    builder.Services.AddScoped<IPagamentosClient, PagamentosClientSimulado>();
}
else
{
    builder.Services.AddHttpClient<IPagamentosClient, PagamentosClient>(client =>
    {
        client.BaseAddress = new Uri(config["Servicos:PagamentosUrl"]!);
        client.Timeout = TimeSpan.FromSeconds(10);
    });
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Cria a tabela "vendas" se ainda não existir
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VendasDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapControllers();

app.Run();