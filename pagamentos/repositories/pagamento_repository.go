package repositories

import (
	"context"
	"database/sql"
	"errors"
	"time"

	"pagamentos/models"
)

// formatoDataHora é o formato ISO 8601 sem fuso que o contrato exige.
// SQLite não tem tipo de data nativo, então guardamos como TEXT.
const formatoDataHora = "2006-01-02T15:04:05"

type PagamentoRepository struct {
	db *sql.DB
}

func NovoPagamentoRepository(db *sql.DB) *PagamentoRepository {
	return &PagamentoRepository{db: db}
}

func (r *PagamentoRepository) GarantirTabela(ctx context.Context) error {
	_, err := r.db.ExecContext(ctx, `
		CREATE TABLE IF NOT EXISTS pagamentos (
			id            INTEGER PRIMARY KEY AUTOINCREMENT,
			venda_id      INTEGER NOT NULL,
			passageiro_id INTEGER NOT NULL,
			valor         REAL    NOT NULL,
			metodo        TEXT    NOT NULL CHECK (metodo IN ('CARTAO', 'PIX')),
			status        TEXT    NOT NULL CHECK (status IN ('APROVADO', 'RECUSADO')),
			data_hora     TEXT    NOT NULL
		)`)
	return err
}

func (r *PagamentoRepository) Criar(ctx context.Context, p models.Pagamento) (models.Pagamento, error) {
	err := r.db.QueryRowContext(ctx,
		`INSERT INTO pagamentos (venda_id, passageiro_id, valor, metodo, status, data_hora)
		 VALUES (?, ?, ?, ?, ?, ?)
		 RETURNING id`,
		p.VendaID,
		p.PassageiroID,
		p.Valor,
		p.Metodo,
		p.Status,
		time.Time(p.DataHora).Format(formatoDataHora),
	).Scan(&p.ID)
	if err != nil {
		return models.Pagamento{}, err
	}
	return p, nil
}

func (r *PagamentoRepository) BuscarPorID(ctx context.Context, id int64) (models.Pagamento, error) {
	var p models.Pagamento
	var dataHora string

	err := r.db.QueryRowContext(ctx,
		`SELECT id, venda_id, passageiro_id, valor, metodo, status, data_hora
		   FROM pagamentos
		  WHERE id = ?`,
		id,
	).Scan(&p.ID, &p.VendaID, &p.PassageiroID, &p.Valor, &p.Metodo, &p.Status, &dataHora)

	if errors.Is(err, sql.ErrNoRows) {
		return models.Pagamento{}, models.ErrNaoEncontrado
	}
	if err != nil {
		return models.Pagamento{}, err
	}

	t, err := time.ParseInLocation(formatoDataHora, dataHora, time.Local)
	if err != nil {
		return models.Pagamento{}, err
	}
	p.DataHora = models.DataHora(t)
	return p, nil
}