package services

import (
	"context"
	"math"
	"time"

	"pagamentos/dtos"
	"pagamentos/models"
)

// Regra fixa de simulação do contrato: valor acima disso é RECUSADO.
const limiteAprovacao = 5000.00

type PagamentoRepo interface {
	Criar(ctx context.Context, p models.Pagamento) (models.Pagamento, error)
	BuscarPorID(ctx context.Context, id int64) (models.Pagamento, error)
}

type PagamentoService struct {
	repo PagamentoRepo
}

func NovoPagamentoService(repo PagamentoRepo) *PagamentoService {
	return &PagamentoService{repo: repo}
}

func (s *PagamentoService) Criar(ctx context.Context, dto dtos.CriarPagamentoRequest) (models.Pagamento, error) {
	if dto.VendaID <= 0 || dto.PassageiroID <= 0 {
		return models.Pagamento{}, models.ErroValidacao{Mensagem: "vendaId e passageiroId devem ser maiores que zero."}
	}
	if dto.Valor <= 0 {
		return models.Pagamento{}, models.ErroValidacao{Mensagem: "O valor deve ser maior que zero."}
	}
	if dto.Metodo != "CARTAO" && dto.Metodo != "PIX" {
		return models.Pagamento{}, models.ErroValidacao{Mensagem: "Método de pagamento inválido (use CARTAO ou PIX)."}
	}

	status := "APROVADO"
	if dto.Valor > limiteAprovacao {
		status = "RECUSADO"
	}

	// RECUSADO também é gravado e retorna 201, conforme o contrato
	return s.repo.Criar(ctx, models.Pagamento{
		VendaID:      dto.VendaID,
		PassageiroID: dto.PassageiroID,
		Valor:        math.Round(dto.Valor*100) / 100,
		Metodo:       dto.Metodo,
		Status:       status,
		DataHora:     models.DataHora(time.Now().Truncate(time.Second)),
	})
}

func (s *PagamentoService) BuscarPorID(ctx context.Context, id int64) (models.Pagamento, error) {
	return s.repo.BuscarPorID(ctx, id)
}