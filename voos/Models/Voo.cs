namespace Voos.Api.Models;

// Entidade que vira tabela no banco
public class Voo
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public DateTime DataPartida { get; set; }
    public DateTime DataChegada { get; set; }
    public decimal Preco { get; set; }
    public int TotalAssentos { get; set; }
    public int AssentosDisponiveis { get; set; }
}