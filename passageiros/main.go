package main

import (
	"context"
	"log"
	"net/http"
	"os"
	"time"

	"passageiros/controllers"
	"passageiros/data"
	"passageiros/middleware"
	"passageiros/repositories"
	"passageiros/services"
)

func main() {
	tursoURL := os.Getenv("TURSO_DATABASE_URL")
	if tursoURL == "" {
		log.Fatal("variável de ambiente TURSO_DATABASE_URL não definida")
	}
	tursoToken := os.Getenv("TURSO_AUTH_TOKEN")
	if tursoToken == "" {
		log.Fatal("variável de ambiente TURSO_AUTH_TOKEN não definida")
	}
	porta := getenv("PORT", "8081")

	ctx, cancel := context.WithTimeout(context.Background(), 15*time.Second)
	defer cancel()

	db, err := data.Conectar(ctx, tursoURL, tursoToken)
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

	// Monta as camadas: repository → service → controller
	repo := repositories.NovoPassageiroRepository(db)
	if err := repo.GarantirTabela(ctx); err != nil {
		log.Fatal("erro ao criar tabela: ", err)
	}
	service := services.NovoPassageiroService(repo)
	controller := controllers.NovoPassageiroController(service)

	mux := http.NewServeMux()
	mux.HandleFunc("POST /passageiros", controller.Criar)
	mux.HandleFunc("GET /passageiros/{id}", controller.BuscarPorID)
	mux.HandleFunc("/", controllers.RotaNaoEncontrada)

	srv := &http.Server{
		Addr:              ":" + porta,
		Handler:           middleware.CORS(mux),
		ReadHeaderTimeout: 5 * time.Second,
	}

	log.Printf("Passageiros rodando na porta %s", porta)
	log.Fatal(srv.ListenAndServe())
}

func getenv(chave, padrao string) string {
	if v := os.Getenv(chave); v != "" {
		return v
	}
	return padrao
}