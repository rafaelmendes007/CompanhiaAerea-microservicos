namespace Vendas.Api.DTOs
{
    public record PassageiroDto(long Id, string Nome, string Cpf, string Email);

    public record VooDto(int Id, string Codigo, string Origem, string Destino,
        DateTime DataPartida, decimal Preco, int AssentosDisponiveis);

    public record PagamentoRequestDto(int VendaId, long PassageiroId, decimal Valor, string Metodo);

    public record PagamentoResponseDto(long Id, string Status);
}
