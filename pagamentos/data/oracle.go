package data

import (
	"context"
	"database/sql"
	"fmt"

	_ "github.com/sijms/go-ora/v2"
)

// Conectar abre o pool de conexões com o Oracle e testa a conexão.
func Conectar(ctx context.Context, dsn string) (*sql.DB, error) {
	db, err := sql.Open("oracle", dsn)
	if err != nil {
		return nil, err
	}
	db.SetMaxOpenConns(10)

	if err := db.PingContext(ctx); err != nil {
		db.Close()
		return nil, fmt.Errorf("não foi possível conectar ao Oracle: %w", err)
	}
	return db, nil
}