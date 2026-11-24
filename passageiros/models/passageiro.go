package models

import "errors"

// ErrNaoEncontrado é o erro de domínio para registros inexistentes.
var ErrNaoEncontrado = errors.New("registro não encontrado")

// ErroValidacao é devolvido pelo service quando os dados são inválidos (→ 400).
type ErroValidacao struct {
	Mensagem string
}

func (e ErroValidacao) Error() string {
	return e.Mensagem
}

type Passageiro struct {
	ID    int64  `json:"id"`
	Nome  string `json:"nome"`
	Cpf   string `json:"cpf"`
	Email string `json:"email"`
}