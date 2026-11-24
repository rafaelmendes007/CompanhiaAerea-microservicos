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

// GarantirTabela cria a tabela PASSAGEIROS se ela ainda não existir.
// No Oracle, o DDL não deve terminar com ponto e vírgula.
func (r *PassageiroRepository) GarantirTabela(ctx context.Context) error {
	var existe int
	err := r.db.QueryRowContext(ctx,
		`SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'PASSAGEIROS'`,
	).Scan(&existe)
	if err != nil {
		return err
	}
	if existe > 0 {
		return nil
	}

	_, err = r.db.ExecContext(ctx, `
		CREATE TABLE PASSAGEIROS (
			ID    NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
			NOME  VARCHAR2(120) NOT NULL,
			CPF   VARCHAR2(11)  NOT NULL,
			EMAIL VARCHAR2(160) NOT NULL
		)`)
	return err
}

func (r *PassageiroRepository) Criar(ctx context.Context, p models.Passageiro) (models.Passageiro, error) {
	var id int64
	_, err := r.db.ExecContext(ctx,
		`INSERT INTO PASSAGEIROS (NOME, CPF, EMAIL)
		 VALUES (:nome, :cpf, :email)
		 RETURNING ID INTO :id`,
		sql.Named("nome", p.Nome),
		sql.Named("cpf", p.Cpf),
		sql.Named("email", p.Email),
		sql.Named("id", sql.Out{Dest: &id}),
	)
	if err != nil {
		return models.Passageiro{}, err
	}
	p.ID = id
	return p, nil
}

func (r *PassageiroRepository) BuscarPorID(ctx context.Context, id int64) (models.Passageiro, error) {
	var p models.Passageiro
	err := r.db.QueryRowContext(ctx,
		`SELECT ID, NOME, CPF, EMAIL FROM PASSAGEIROS WHERE ID = :id`,
		sql.Named("id", id),
	).Scan(&p.ID, &p.Nome, &p.Cpf, &p.Email)

	if errors.Is(err, sql.ErrNoRows) {
		return models.Passageiro{}, models.ErrNaoEncontrado
	}
	return p, err
}