using Vendas.Api.DTOs;

// Usado enquanto o serviço real de Passageiros não estiver no ar.
// Qualquer id existe, exceto 999 (para testar o "não encontrado").
namespace Vendas.Api.Clients.Simulados
{
    public class PassageirosClientSimulado : IPassageirosClient
    {
        public Task<PassageiroDto?> ObterPorIdAsync(long id)
        {
            if (id == 999)
                return Task.FromResult<PassageiroDto?>(null);

            var passageiro = new PassageiroDto(id, $"Passageiro Teste {id}", "00000000000", $"passageiro{id}@teste.com");
            return Task.FromResult<PassageiroDto?>(passageiro);
        }
    }
}
