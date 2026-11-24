package controllers

import (
	"encoding/json"
	"errors"
	"log"
	"net/http"

	"passageiros/models"
)

// tratarErro converte o erro do service em status HTTP e JSON { "mensagem": "..." }.
func tratarErro(w http.ResponseWriter, err error, msgNaoEncontrado string) {
	var validacao models.ErroValidacao

	switch {
	case errors.As(err, &validacao):
		responderErro(w, http.StatusBadRequest, validacao.Mensagem)
	case errors.Is(err, models.ErrNaoEncontrado):
		responderErro(w, http.StatusNotFound, msgNaoEncontrado)
	default:
		// Erro real do Oracle: registra no log e devolve 503 (contrato: serviço dependente fora do ar)
		log.Printf("erro de banco: %v", err)
		responderErro(w, http.StatusServiceUnavailable, "Banco de dados indisponível.")
	}
}

func responderJSON(w http.ResponseWriter, status int, v any) {
	w.Header().Set("Content-Type", "application/json; charset=utf-8")
	w.WriteHeader(status)
	_ = json.NewEncoder(w).Encode(v)
}

func responderErro(w http.ResponseWriter, status int, msg string) {
	responderJSON(w, status, map[string]string{"mensagem": msg})
}

func RotaNaoEncontrada(w http.ResponseWriter, r *http.Request) {
	responderErro(w, http.StatusNotFound, "Rota não encontrada.")
}