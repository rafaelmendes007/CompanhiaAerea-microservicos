from database import SessionLocal
from models import Checkin
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import os
import httpx
from fastapi.responses import JSONResponse
from datetime import datetime
from sqlalchemy.exc import IntegrityError
from database import Base, engine
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)
Base.metadata.create_all(bind=engine)

@app.exception_handler(HTTPException)
def tratar_erro_http(request, erro):
    return JSONResponse(
        status_code=erro.status_code,
        content={"mensagem": erro.detail}
    )


class CheckinEntrada(BaseModel):
    passageiroId: int
    vooId: int


@app.get("/")
def inicio():
    return {"mensagem": "Serviço de check-in funcionando!"}

def consultar_servico(url_base, recurso, id_recurso):
    if not url_base:
        raise HTTPException(
            status_code=503,
            detail=f"Endereço do serviço de {recurso} não configurado"
        )

    try:
        resposta = httpx.get(
            f"{url_base}/{recurso}/{id_recurso}",
            timeout=5.0
        )
    except httpx.RequestError:
        raise HTTPException(
            status_code=503,
            detail=f"Serviço de {recurso} indisponível"
        )

    if resposta.status_code == 404:
        raise HTTPException(
            status_code=404,
            detail=f"Registro não encontrado em {recurso}"
        )

    if resposta.status_code != 200:
        raise HTTPException(
            status_code=503,
            detail=f"Erro ao consultar o serviço de {recurso}"
        )

    return resposta.json()


@app.post("/checkins", status_code=201)
def criar_checkin(dados: CheckinEntrada):
    consultar_servico(
        os.getenv("PASSAGEIROS_URL"),
        "passageiros",
        dados.passageiroId
    )

    voo = consultar_servico(
        os.getenv("VOOS_URL"),
        "voos",
        dados.vooId
    )

    banco = SessionLocal()

    try:
        checkin_existente = (
            banco.query(Checkin)
            .filter(
                Checkin.passageiro_id == dados.passageiroId,
                Checkin.voo_id == dados.vooId
            )
            .first()
        )

        if checkin_existente:
            raise HTTPException(
                status_code=409,
                detail="Passageiro já fez check-in neste voo"
            )

        assentos_ocupados = {
            item.assento
            for item in banco.query(Checkin)
            .filter(Checkin.voo_id == dados.vooId)
            .all()
        }

        assento_escolhido = None

        # Organiza os assentos em fileiras de até seis lugares: A a F.
        for numero in range(voo["totalAssentos"]):
            fileira = numero // 6 + 1
            letra = "ABCDEF"[numero % 6]
            assento = f"{fileira}{letra}"

            if assento not in assentos_ocupados:
                assento_escolhido = assento
                break

        if assento_escolhido is None:
            raise HTTPException(
                status_code=409,
                detail="Não há assentos disponíveis para check-in"
            )

        novo_checkin = Checkin(
            passageiro_id=dados.passageiroId,
            voo_id=dados.vooId,
            assento=assento_escolhido,
            data_hora=datetime.now()
        )

        banco.add(novo_checkin)

        try:
            banco.commit()
        except IntegrityError:
            banco.rollback()
            raise HTTPException(
                status_code=409,
                detail="Check-in ou assento já registrado. Tente novamente."
            )

        banco.refresh(novo_checkin)

        return {
            "id": novo_checkin.id,
            "passageiroId": novo_checkin.passageiro_id,
            "vooId": novo_checkin.voo_id,
            "assento": novo_checkin.assento,
            "dataHora": novo_checkin.data_hora.isoformat()
        }
    finally:
        banco.close()

@app.get("/checkins")
def listar_checkins(vooId: int):
    banco = SessionLocal()

    try:
        checkins = banco.query(Checkin).filter(Checkin.voo_id == vooId).all()

        return [
            {
                "id": checkin.id,
                "passageiroId": checkin.passageiro_id,
                "vooId": checkin.voo_id,
                "assento": checkin.assento,
                "dataHora": checkin.data_hora.isoformat()
            }
            for checkin in checkins
        ]
    finally:
        banco.close()

        
@app.get("/checkins/{checkin_id}")
def consultar_checkin(checkin_id: int):
    banco = SessionLocal()

    try:
        checkin = banco.query(Checkin).filter(Checkin.id == checkin_id).first()

        if checkin is None:
            raise HTTPException(
                status_code=404,
                detail="Check-in não encontrado"
            )

        return {
            "id": checkin.id,
            "passageiroId": checkin.passageiro_id,
            "vooId": checkin.voo_id,
            "assento": checkin.assento,
            "dataHora": checkin.data_hora.isoformat()
        }
    finally:
        banco.close()