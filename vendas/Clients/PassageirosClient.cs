using Vendas.Api.DTOs;

namespace Vendas.Api.Clients
{
    public class PassageirosClient : IPassageirosClient
    {
        private readonly HttpClient _http;

        public PassageirosClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<PassageiroDto?> ObterPorIdAsync(long id)
        {
            try
            {
                var resposta = await _http.GetAsync($"/passageiro/{id}");

                if (resposta.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                resposta.EnsureSuccessStatusCode(); // Qualquer outro erro vira exceção
                return await resposta.Content.ReadFromJsonAsync<PassageiroDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new ServicoIndisponivelException("Passgeiros", ex);
            }
        }
    }
}
