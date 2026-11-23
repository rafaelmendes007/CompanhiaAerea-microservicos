namespace Voos.Api.DTOs
{
    // Formato JSON que o cliente envia pra cadastrar um voo
    public record CriarVooDto(
        string Codigo,
        string Origem,
        string Destino,
        DateTime DataPartida,
        DateTime DataChegada,
        decimal Preco,
        int TotalAssentos);
}
