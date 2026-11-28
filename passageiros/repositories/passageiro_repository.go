package repositories

import (
	"context"
	"database/sql"
	"errors"

	"passageiros/models"
)

type PassageiroRepository struct {
	db *sql.DB
}

func NovoPassageiroRepository(db *sql.DB) *PassageiroRepository {
	return &PassageiroRepository{db: db}
}

// GarantirTabela cria a tabela passageiros se ela ainda não existir.
// SQLite não tem GENERATED ALWAYS AS IDENTITY, então usamos INTEGER PRIMARY KEY AUTOINCREMENT.
func (r *PassageiroRepository) GarantirTabela(ctx context.Context) error {
	_, err := r.db.ExecContext(ctx, `
		CREATE TABLE IF NOT EXISTS passageiros (
			id    INTEGER PRIMARY KEY AUTOINCREMENT,
			nome  TEXT NOT NULL,
			cpf   TEXT NOT NULL,
			email TEXT NOT NULL
		)`)
	return err
}

func (r *PassageiroRepository) Criar(ctx context.Context, p models.Passageiro) (models.Passageiro, error) {
	err := r.db.QueryRowContext(ctx,
		`INSERT INTO passageiros (nome, cpf, email)
		 VALUES (?, ?, ?)
		 RETURNING id`,
		p.Nome, p.Cpf, p.Email,
	).Scan(&p.ID)
	if err != nil {
		return models.Passageiro{}, err
	}
	return p, nil
}

func (r *PassageiroRepository) BuscarPorID(ctx context.Context, id int64) (models.Passageiro, error) {
	var p models.Passageiro
	err := r.db.QueryRowContext(ctx,
		`SELECT id, nome, cpf, email FROM passageiros WHERE id = ?`,
		id,
	).Scan(&p.ID, &p.Nome, &p.Cpf, &p.Email)

	if errors.Is(err, sql.ErrNoRows) {
		return models.Passageiro{}, models.ErrNaoEncontrado
	}
	if err != nil {
		return models.Passageiro{}, err
	}
	return p, nil
}