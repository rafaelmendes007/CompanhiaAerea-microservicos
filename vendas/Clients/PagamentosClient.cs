using Vendas.Api.DTOs;


namespace Vendas.Api.Clients
{
    public class PagamentosClient : IPagamentosClient
    {
        private readonly HttpClient _http;

        public PagamentosClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<PagamentoResponseDto> ProcessarAsync(PagamentoRequestDto pedido)
        {
            try
            {
                var resposta = await _http.PostAsJsonAsync("/pagamentos", pedido);
                resposta.EnsureSuccessStatusCode();

                return await resposta.Content.ReadFromJsonAsync<PagamentoResponseDto>()
                    ?? throw new ServicoIndisponivelException("Pagamentos");
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                throw new ServicoIndisponivelException("Pagamentos", ex);
            }
        }
    }
}
