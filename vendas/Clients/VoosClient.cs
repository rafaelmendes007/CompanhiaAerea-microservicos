using System.Net;
using Vendas.Api.DTOs;

namespace Vendas.Api.Clients
{
    public class VoosClient : IVoosClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<VoosClient> _logger;

        public VoosClient(HttpClient http, ILogger<VoosClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<VooDto?> ObterPorIdAsync(int id)
        {
            try
            {
                var resposta = await _http.GetAsync($"/voos/{id}");

                if (resposta.StatusCode == HttpStatusCode.NotFound)
                    return null;

                resposta.EnsureSuccessStatusCode();
                return await resposta.Content.ReadFromJsonAsync<VooDto>();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new ServicoIndisponivelException("Voos", ex);
            }
        }

        public async Task<ResultadoReserva> ReservarAssentoAsync(int id)
        {
            try
            {
                var resposta = await _http.PostAsync($"/voos/{id}/reservar-assento", null);

                if (resposta.StatusCode == HttpStatusCode.Conflict)
                    return ResultadoReserva.Lotado;
                if (resposta.StatusCode == HttpStatusCode.NotFound)
                    return ResultadoReserva.VooNaoEncontrado;

                resposta.EnsureSuccessStatusCode();
                return ResultadoReserva.Sucesso;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new ServicoIndisponivelException("Voos", ex);
            }
        }

        // "Desfazer" a reserva. Se falhar, só registra no log:
        // a venda já está sendo recusada e não queremos esconder o erro original.
        public async Task LiberarAssentoAsync(int id)
        {
            try
            {
                var resposta = await _http.PostAsync($"/voos/{id}/liberar-assento", null);
                resposta.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Não foi possível liberar o assento do voo {VooId}", id);
            }
        }
    }
}
