package dtos

// CriarPagamentoRequest é o JSON que Vendas envia em POST /pagamentos.
type CriarPagamentoRequest struct {
	VendaID      int64   `json:"vendaId"`
	PassageiroID int64   `json:"passageiroId"`
	Valor        float64 `json:"valor"`
	Metodo       string  `json:"metodo"`
}