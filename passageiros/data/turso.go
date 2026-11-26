package data

import (
	"context"
	"database/sql"
	"fmt"

	_ "github.com/WillowWorks-io/libsql-client-go/libsql"
)

// Conectar abre o pool de conexões com o banco Turso e testa a conexão.
// url é a URL do banco (libsql://...) e token é o token de acesso.
func Conectar(ctx context.Context, url, token string) (*sql.DB, error) {
	db, err := sql.Open("libsql", url+"?authToken="+token)
	if err != nil {
		return nil, err
	}
	db.SetMaxOpenConns(10)

	if err := db.PingContext(ctx); err != nil {
		db.Close()
		return nil, fmt.Errorf("não foi possível conectar ao Turso: %w", err)
	}
	return db, nil
}