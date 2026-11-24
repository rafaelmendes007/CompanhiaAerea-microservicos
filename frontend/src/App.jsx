import { useState } from "react";
import "./App.css";

const PASSAGEIROS_URL = import.meta.env.VITE_PASSAGEIROS_URL || "http://127.0.0.1:8081";
const VOOS_URL = import.meta.env.VITE_VOOS_URL || "http://127.0.0.1:8083";
const VENDAS_URL = import.meta.env.VITE_VENDAS_URL || "http://127.0.0.1:8084";
const CHECKIN_URL = import.meta.env.VITE_CHECKIN_URL || "http://127.0.0.1:8085";

function App() {
  const [pagina, setPagina] = useState("inicio");
  const [passageiroId, setPassageiroId] = useState("");
  const [vooId, setVooId] = useState("");
  const [mensagem, setMensagem] = useState("");
  const [checkinId, setCheckinId] = useState("");
  const [resultadoConsulta, setResultadoConsulta] = useState(null);
  const [erroConsulta, setErroConsulta] = useState("");
  const [nome, setNome] = useState("");
  const [cpf, setCpf] = useState("");
  const [email, setEmail] = useState("");
  const [mensagemPassageiro, setMensagemPassageiro] = useState("");
  const [origem, setOrigem] = useState("");
  const [destino, setDestino] = useState("");
  const [voos, setVoos] = useState([]);
  const [mensagemVoos, setMensagemVoos] = useState("");
  const [compraPassageiroId, setCompraPassageiroId] = useState("");
  const [compraVooId, setCompraVooId] = useState("");
  const [metodoPagamento, setMetodoPagamento] = useState("CARTAO");
  const [mensagemCompra, setMensagemCompra] = useState("");
  const [consultaPassageiroId, setConsultaPassageiroId] = useState("");
  const [compras, setCompras] = useState([]);
  const [mensagemConsultaCompras, setMensagemConsultaCompras] = useState("");

  async function consultarCompras(evento) {
    evento.preventDefault();
    setCompras([]);
    setMensagemConsultaCompras("Buscando compras...");
  
    try {
      const resposta = await fetch(
        `http://127.0.0.1:8084/vendas?passageiroId=${consultaPassageiroId}`
      );
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setMensagemConsultaCompras(
          dados.mensagem || "Não foi possível consultar as compras."
        );
        return;
      }
  
      setCompras(dados);
      setMensagemConsultaCompras(
        dados.length === 0 ? "Nenhuma compra encontrada." : ""
      );
    } catch {
      setMensagemConsultaCompras("Não foi possível conectar ao serviço de vendas.");
    }
  }
  
  async function comprarPassagem(evento) {
    evento.preventDefault();
    setMensagemCompra("Processando compra...");
  
    try {
      const resposta = await fetch("http://127.0.0.1:8084/vendas", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          passageiroId: Number(compraPassageiroId),
          vooId: Number(compraVooId),
          metodoPagamento: metodoPagamento,
        }),
      });
  
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setMensagemCompra(dados.mensagem || "Não foi possível concluir a compra.");
        return;
      }
  
      setMensagemCompra(`Compra confirmada! Número da venda: ${dados.id}`);
    } catch {
      setMensagemCompra("Não foi possível conectar ao serviço de vendas.");
    }
  }

  async function buscarVoos(evento) {
    evento.preventDefault();
    setMensagemVoos("Buscando voos...");
    setVoos([]);
  
    const filtros = new URLSearchParams({
      origem: origem.toUpperCase(),
      destino: destino.toUpperCase(),
    });
  
    try {
      const resposta = await fetch(
        `http://127.0.0.1:8083/voos?${filtros}`
      );
  
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setMensagemVoos(dados.mensagem || "Não foi possível buscar voos.");
        return;
      }
  
      setVoos(dados);
      setMensagemVoos(dados.length === 0 ? "Nenhum voo encontrado." : "");
    } catch {
      setMensagemVoos("Não foi possível conectar ao serviço de voos.");
    }
  }

  async function cadastrarPassageiro(evento) {
    evento.preventDefault();
    setMensagemPassageiro("Cadastrando passageiro...");
  
    try {
      const resposta = await fetch("http://127.0.0.1:8081/passageiros", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ nome, cpf, email }),
      });
  
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setMensagemPassageiro(dados.mensagem || "Não foi possível cadastrar.");
        return;
      }
  
      setMensagemPassageiro(`Passageiro cadastrado! Número: ${dados.id}`);
      setNome("");
      setCpf("");
      setEmail("");
    } catch {
      setMensagemPassageiro("Não foi possível conectar ao serviço de passageiros.");
    }
  }


  async function consultarCheckin(evento) {
    evento.preventDefault();
    setResultadoConsulta(null);
    setErroConsulta("");
  
    try {
      const resposta = await fetch(
        `${CHECKIN_URL}/checkins/${checkinId}`
      );
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setErroConsulta(dados.mensagem || "Não foi possível consultar.");
        return;
      }
  
      setResultadoConsulta(dados);
    } catch {
      setErroConsulta("Não foi possível conectar ao serviço de check-in.");
    }
  }

  async function fazerCheckin(evento) {
    evento.preventDefault();
    setMensagem("Enviando check-in...");
  
    try {
      const resposta = await fetch(`${CHECKIN_URL}/checkins`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          passageiroId: Number(passageiroId),
          vooId: Number(vooId),
        }),
      });
  
      const dados = await resposta.json();
  
      if (!resposta.ok) {
        setMensagem(dados.mensagem || "Não foi possível fazer o check-in.");
        return;
      }
  
      setMensagem(`Check-in realizado! Seu assento é ${dados.assento}.`);
    } catch {
      setMensagem("Não foi possível conectar ao serviço de check-in.");
    }
  }

  return (
    <div className="app">
      <header className="cabecalho">
        <div>
          <h1>Companhia Aérea</h1>
          <p>Seu sistema de viagens</p>
        </div>

        <nav>
          <button onClick={() => setPagina("inicio")}>Início</button>
          <button onClick={() => setPagina("passageiros")}>Passageiros</button>
          <button onClick={() => setPagina("voos")}>Voos</button>
          <button onClick={() => setPagina("compras")}>Compras</button>
          <button onClick={() => setPagina("checkin")}>Check-in</button>
        </nav>
      </header>

      <main>
        {pagina === "inicio" && (
          <section>
            <h2>Bem-vindo a bordo!</h2>
            <p>
              Aqui você poderá buscar voos, comprar passagens e realizar
              seu check-in.
            </p>
          </section>
        )}

       {pagina === "passageiros" && (
  <section>
    <h2>Cadastrar passageiro</h2>
    <p>Preencha os dados para cadastrar um passageiro.</p>

    <form onSubmit={cadastrarPassageiro}>
      <div>
        <label htmlFor="nome">Nome</label>
        <input
          id="nome"
          type="text"
          value={nome}
          onChange={(evento) => setNome(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="cpf">CPF</label>
        <input
          id="cpf"
          type="text"
          value={cpf}
          onChange={(evento) => setCpf(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="email">E-mail</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(evento) => setEmail(evento.target.value)}
          required
        />
      </div>

      <button type="submit">Cadastrar passageiro</button>
    </form>

    {mensagemPassageiro && <p>{mensagemPassageiro}</p>}
  </section>
)}

{pagina === "voos" && (
  <section>
    <h2>Buscar voos</h2>
    <p>Informe os códigos de origem e destino. Exemplo: FOR e GRU.</p>

    <form onSubmit={buscarVoos}>
      <div>
        <label htmlFor="origem">Origem</label>
        <input
          id="origem"
          type="text"
          value={origem}
          onChange={(evento) => setOrigem(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="destino">Destino</label>
        <input
          id="destino"
          type="text"
          value={destino}
          onChange={(evento) => setDestino(evento.target.value)}
          required
        />
      </div>

      <button type="submit">Buscar voos</button>
    </form>

    {mensagemVoos && <p>{mensagemVoos}</p>}

    {voos.map((voo) => (
      <div key={voo.id}>
        <h3>Voo {voo.codigo}</h3>
        <p>Número do voo: {voo.id}</p>
        <p>{voo.origem} → {voo.destino}</p>
        <p>Partida: {voo.dataPartida}</p>
        <p>Preço: R$ {voo.preco}</p>
        <p>Assentos disponíveis: {voo.assentosDisponiveis}</p>
      </div>
    ))}
  </section>
)}

        {pagina === "compras" && (
  <section>
    <h2>Comprar passagem</h2>
    <p>Informe o passageiro, o voo e a forma de pagamento.</p>

    <form onSubmit={comprarPassagem}>
      <div>
        <label htmlFor="compraPassageiroId">Número do passageiro</label>
        <input
          id="compraPassageiroId"
          type="number"
          min="1"
          value={compraPassageiroId}
          onChange={(evento) => setCompraPassageiroId(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="compraVooId">Número do voo</label>
        <input
          id="compraVooId"
          type="number"
          min="1"
          value={compraVooId}
          onChange={(evento) => setCompraVooId(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="metodoPagamento">Forma de pagamento</label>
        <select
          id="metodoPagamento"
          value={metodoPagamento}
          onChange={(evento) => setMetodoPagamento(evento.target.value)}
        >
          <option value="CARTAO">Cartão</option>
          <option value="PIX">Pix</option>
        </select>
      </div>

      <button type="submit">Comprar passagem</button>
    </form>

    {mensagemCompra && <p>{mensagemCompra}</p>}
    <hr />

<h2>Minhas compras</h2>

<form onSubmit={consultarCompras}>
  <label htmlFor="consultaPassageiroId">Número do passageiro</label>
  <input
    id="consultaPassageiroId"
    type="number"
    min="1"
    value={consultaPassageiroId}
    onChange={(evento) => setConsultaPassageiroId(evento.target.value)}
    required
  />
  <button type="submit">Consultar compras</button>
</form>

{mensagemConsultaCompras && <p>{mensagemConsultaCompras}</p>}

{compras.map((compra) => (
  <div key={compra.id}>
    <h3>Compra {compra.id}</h3>
    <p>Voo: {compra.vooId}</p>
    <p>Valor: R$ {compra.valor}</p>
    <p>Status: {compra.status}</p>
  </div>
))}
  </section>
)}
{pagina === "checkin" && (
  <section>
    <h2>Fazer check-in</h2>
    <p>Informe o número do passageiro e o número do voo.</p>

    <form onSubmit={fazerCheckin}>
      <div>
        <label htmlFor="passageiroId">Número do passageiro</label>
        <input
          id="passageiroId"
          type="number"
          min="1"
          value={passageiroId}
          onChange={(evento) => setPassageiroId(evento.target.value)}
          required
        />
      </div>

      <div>
        <label htmlFor="vooId">Número do voo</label>
        <input
          id="vooId"
          type="number"
          min="1"
          value={vooId}
          onChange={(evento) => setVooId(evento.target.value)}
          required
        />
      </div>

      <button type="submit">Fazer check-in</button>
    </form>
    {mensagem && <p>{mensagem}</p>}
    <hr />

<h2>Consultar check-in</h2>

<form onSubmit={consultarCheckin}>
  <label htmlFor="checkinId">Número do check-in</label>
  <input
    id="checkinId"
    type="number"
    min="1"
    value={checkinId}
    onChange={(evento) => setCheckinId(evento.target.value)}
    required
  />
  <button type="submit">Consultar</button>
</form>

{erroConsulta && <p>{erroConsulta}</p>}

{resultadoConsulta && (
  <div>
    <h3>Dados do check-in</h3>
    <p>Número: {resultadoConsulta.id}</p>
    <p>Passageiro: {resultadoConsulta.passageiroId}</p>
    <p>Voo: {resultadoConsulta.vooId}</p>
    <p>Assento: {resultadoConsulta.assento}</p>
    <p>Data e hora: {resultadoConsulta.dataHora}</p>
  </div>
)}
  </section>
)}

      </main>
    </div>
  );
}

export default App;