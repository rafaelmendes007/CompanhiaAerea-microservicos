from fastapi import FastAPI, HTTPException

app = FastAPI()


@app.get("/passageiros/{passageiro_id}")
def buscar_passageiro(passageiro_id: int):
    if passageiro_id not in [1, 2]:
        raise HTTPException(status_code=404, detail="Passageiro não encontrado")

    return {
        "id": passageiro_id,
        "nome": "Passageiro de teste",
        "cpf": "12345678900",
        "email": "teste@email.com"
    }


@app.get("/voos/{voo_id}")
def buscar_voo(voo_id: int):
    if voo_id != 1:
        raise HTTPException(status_code=404, detail="Voo não encontrado")

    return {
        "id": 1,
        "codigo": "AE1001",
        "origem": "FOR",
        "destino": "GRU",
        "totalAssentos": 120
    }