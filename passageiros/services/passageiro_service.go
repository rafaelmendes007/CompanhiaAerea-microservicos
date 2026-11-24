package services

import (
	"context"
	"net/mail"
	"regexp"
	"strings"

	"passageiros/dtos"
	"passageiros/models"
)

var cpfRegex = regexp.MustCompile(`^\d{11}$`)

// PassageiroRepo é o contrato que o service exige do repositório.
// Fica aqui (e não no pacote repositories) para o service não depender do banco.
type PassageiroRepo interface {
	Criar(ctx context.Context, p models.Passageiro) (models.Passageiro, error)
	BuscarPorID(ctx context.Context, id int64) (models.Passageiro, error)
}

type PassageiroService struct {
	repo PassageiroRepo
}

func NovoPassageiroService(repo PassageiroRepo) *PassageiroService {
	return &PassageiroService{repo: repo}
}

func (s *PassageiroService) Criar(ctx context.Context, dto dtos.CriarPassageiroRequest) (models.Passageiro, error) {
	nome := strings.TrimSpace(dto.Nome)
	cpf := strings.TrimSpace(dto.Cpf)
	email := strings.TrimSpace(dto.Email)

	if nome == "" {
		return models.Passageiro{}, models.ErroValidacao{Mensagem: "O nome é obrigatório."}
	}
	if !cpfRegex.MatchString(cpf) {
		return models.Passageiro{}, models.ErroValidacao{Mensagem: "O CPF deve conter 11 dígitos numéricos."}
	}
	if _, err := mail.ParseAddress(email); err != nil {
		return models.Passageiro{}, models.ErroValidacao{Mensagem: "E-mail inválido."}
	}

	return s.repo.Criar(ctx, models.Passageiro{Nome: nome, Cpf: cpf, Email: email})
}

func (s *PassageiroService) BuscarPorID(ctx context.Context, id int64) (models.Passageiro, error) {
	return s.repo.BuscarPorID(ctx, id)
}