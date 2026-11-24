package controllers

import (
	"encoding/json"
	"net/http"
	"strconv"

	"pagamentos/dtos"
	"pagamentos/services"
)

type PagamentoController struct {
	service *services.PagamentoService
}

func NovoPagamentoController(service *services.PagamentoService) *PagamentoController {
	return &PagamentoController{service: service}
}

// POST /pagamentos → 201 Pagamento (inclusive RECUSADO) · 400
func (c *PagamentoController) Criar(w http.ResponseWriter, r *http.Request) {
	var dto dtos.CriarPagamentoRequest
	if err := json.NewDecoder(http.MaxBytesReader(w, r.Body, 1<<20)).Decode(&dto); err != nil {
		responderErro(w, http.StatusBadRequest, "Corpo da requisição inválido.")
		return
	}

	p, err := c.service.Criar(r.Context(), dto)
	if err != nil {
		tratarErro(w, err, "Pagamento não encontrado.")
		return
	}
	responderJSON(w, http.StatusCreated, p)
}

// GET /pagamentos/{id} → 200 Pagamento · 404
func (c *PagamentoController) BuscarPorID(w http.ResponseWriter, r *http.Request) {
	id, err := strconv.ParseInt(r.PathValue("id"), 10, 64)
	if err != nil {
		responderErro(w, http.StatusNotFound, "Pagamento não encontrado.")
		return
	}

	p, err := c.service.BuscarPorID(r.Context(), id)
	if err != nil {
		tratarErro(w, err, "Pagamento não encontrado.")
		return
	}
	responderJSON(w, http.StatusOK, p)
}