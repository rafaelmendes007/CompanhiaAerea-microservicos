package repositories

import (
	"context"
	"database/sql"
	"errors"
	"time"

	"pagamentos/models"
)

type PagamentoRepository struct {
	db *sql.DB
}

func NovoPagamentoRepository(db *sql.DB) *PagamentoRepository {
	return &PagamentoRepository{db: db}
}

func (r *PagamentoRepository) GarantirTabela(ctx context.Context) error {
	var existe int
	err := r.db.QueryRowContext(ctx,
		`SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'PAGAMENTOS'`,
	).Scan(&existe)
	if err != nil {
		return err
	}
	if existe > 0 {
		return nil
	}

	_, err = r.db.ExecContext(ctx, `
		CREATE TABLE PAGAMENTOS (
			ID            NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
			VENDA_ID      NUMBER        NOT NULL,
			PASSAGEIRO_ID NUMBER        NOT NULL,
			VALOR         NUMBER(10,2)  NOT NULL,
			METODO        VARCHAR2(10)  NOT NULL,
			STATUS        VARCHAR2(10)  NOT NULL,
			DATA_HORA     TIMESTAMP     NOT NULL,
			CONSTRAINT ck_pag_metodo CHECK (METODO IN ('CARTAO', 'PIX')),
			CONSTRAINT ck_pag_status CHECK (STATUS IN ('APROVADO', 'RECUSADO'))
		)`)
	return err
}

func (r *PagamentoRepository) Criar(ctx context.Context, p models.Pagamento) (models.Pagamento, error) {
	var id int64
	_, err := r.db.ExecContext(ctx,
		`INSERT INTO PAGAMENTOS (VENDA_ID, PASSAGEIRO_ID, VALOR, METODO, STATUS, DATA_HORA)
		 VALUES (:venda, :passageiro, :valor, :metodo, :status, :data)
		 RETURNING ID INTO :id`,
		sql.Named("venda", p.VendaID),
		sql.Named("passageiro", p.PassageiroID),
		sql.Named("valor", p.Valor),
		sql.Named("metodo", p.Metodo),
		sql.Named("status", p.Status),
		sql.Named("data", time.Time(p.DataHora)),
		sql.Named("id", sql.Out{Dest: &id}),
	)
	if err != nil {
		return models.Pagamento{}, err
	}
	p.ID = id
	return p, nil
}

func (r *PagamentoRepository) BuscarPorID(ctx context.Context, id int64) (models.Pagamento, error) {
	var p models.Pagamento
	var dataHora time.Time

	err := r.db.QueryRowContext(ctx,
		`SELECT ID, VENDA_ID, PASSAGEIRO_ID, VALOR, METODO, STATUS, DATA_HORA
		   FROM PAGAMENTOS
		  WHERE ID = :id`,
		sql.Named("id", id),
	).Scan(&p.ID, &p.VendaID, &p.PassageiroID, &p.Valor, &p.Metodo, &p.Status, &dataHora)

	if errors.Is(err, sql.ErrNoRows) {
		return models.Pagamento{}, models.ErrNaoEncontrado
	}
	if err != nil {
		return models.Pagamento{}, err
	}
	p.DataHora = models.DataHora(dataHora)
	return p, nil
}