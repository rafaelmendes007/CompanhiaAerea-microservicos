from sqlalchemy import Column, DateTime, Integer, String, UniqueConstraint
from database import Base


class Checkin(Base):
    __tablename__ = "checkins"

    id = Column(Integer, primary_key=True, autoincrement=True)
    passageiro_id = Column(Integer, nullable=False)
    voo_id = Column(Integer, nullable=False)
    assento = Column(String(10), nullable=False)
    data_hora = Column(DateTime, nullable=False)

    __table_args__ = (
    UniqueConstraint("passageiro_id", "voo_id", name="uq_passageiro_voo"),
    UniqueConstraint("voo_id", "assento", name="uq_voo_assento"),
)