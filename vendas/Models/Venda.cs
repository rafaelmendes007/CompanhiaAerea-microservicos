namespace Vendas.Api.Models
{
    public class Venda
    {
        public int Id {  get; set; }
        public long PassageiroId {  get; set; }
        public int VooId { get; set; }
        public decimal Valor {  get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
        public long? PagamentoId { get; set; }
        public string Status { get; set; } = StatusVenda.Pendente;
        public DateTime DataCompra {  get; set; }
    }

    public static class StatusVenda
    {
        public const string pendente = "PENDENTE";
        public const string confirmada = "CONFIRMADA";
        public const string Recusada = "RECUSADA";
    }
}
