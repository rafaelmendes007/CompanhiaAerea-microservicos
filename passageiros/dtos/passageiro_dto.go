package dtos

// CriarPassageiroRequest é o JSON que o cliente envia em POST /passageiros.
type CriarPassageiroRequest struct {
	Nome  string `json:"nome"`
	Cpf   string `json:"cpf"`
	Email string `json:"email"`
}