package controllers

import (
	"encoding/json"
	"net/http"
	"strconv"

	"passageiros/dtos"
	"passageiros/services"
)

type PassageiroController struct {
	service *services.PassageiroService
}

func NovoPassageiroController(service *services.PassageiroService) *PassageiroController {
	return &PassageiroController{service: service}
}

// POST /passageiros → 201 Passageiro · 400
func (c *PassageiroController) Criar(w http.ResponseWriter, r *http.Request) {
	var dto dtos.CriarPassageiroRequest
	if err := json.NewDecoder(http.MaxBytesReader(w, r.Body, 1<<20)).Decode(&dto); err != nil {
		responderErro(w, http.StatusBadRequest, "Corpo da requisição inválido.")
		return
	}

	p, err := c.service.Criar(r.Context(), dto)
	if err != nil {
		tratarErro(w, err, "Passageiro não encontrado.")
		return
	}
	responderJSON(w, http.StatusCreated, p)
}

// GET /passageiros/{id} → 200 Passageiro · 404
func (c *PassageiroController) BuscarPorID(w http.ResponseWriter, r *http.Request) {
	id, err := strconv.ParseInt(r.PathValue("id"), 10, 64)
	if err != nil {
		responderErro(w, http.StatusNotFound, "Passageiro não encontrado.")
		return
	}

	p, err := c.service.BuscarPorID(r.Context(), id)
	if err != nil {
		tratarErro(w, err, "Passageiro não encontrado.")
		return
	}
	responderJSON(w, http.StatusOK, p)
}