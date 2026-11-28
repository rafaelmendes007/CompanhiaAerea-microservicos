package models

import (
	"errors"
	"time"
)

var ErrNaoEncontrado = errors.New("registro não encontrado")

type ErroValidacao struct {
	Mensagem string
}

func (e ErroValidacao) Error() string {
	return e.Mensagem
}

// DataHora serializa no formato ISO 8601 sem fuso: "2026-11-23T18:30:00"
type DataHora time.Time

func (d DataHora) MarshalJSON() ([]byte, error) {
	t := time.Time(d)
	return []byte(`"` + t.Format("2006-01-02T15:04:05") + `"`), nil
}

type Pagamento struct {
	ID           int64    `json:"id"`
	VendaID      int64    `json:"vendaId"`
	PassageiroID int64    `json:"passageiroId"`
	Valor        float64  `json:"valor"`
	Metodo       string   `json:"metodo"`
	Status       string   `json:"status"`
	DataHora     DataHora `json:"dataHora"`
}