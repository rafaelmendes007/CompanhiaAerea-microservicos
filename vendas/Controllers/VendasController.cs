using Microsoft.AspNetCore.Mvc;
using Vendas.Api.DTOs;
using Vendas.Api.Models;
using Vendas.Api.Services;

namespace Vendas.Api.Controllers
{
    [ApiController]
    [Route("vendas")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaService _service;

        public VendasController(IVendaService service)
        {
            _service = service;
        }

        // POST /vendas  { "passageiroId": 1, "vooId": 1, "metodoPagamento": "CARTAO" }
        [HttpPost]
        public async Task<IActionResult> Criar(CriarVendaDto dto)
        {
            var resultado = await _service.CriarAsync(dto);

            switch (resultado.Tipo)
            {
                case TipoResultadoVenda.Confirmada:
                    return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Venda!.Id }, resultado.Venda);

                case TipoResultadoVenda.PagamentoRecusado:
                    return StatusCode(StatusCodes.Status402PaymentRequired,
                        new { mensagem = resultado.Mensagem, venda = resultado.Venda });

                case TipoResultadoVenda.PassageiroNaoEncontrado:
                case TipoResultadoVenda.VooNaoEncontrado:
                    return NotFound(new { mensagem = resultado.Mensagem });

                case TipoResultadoVenda.VooLotado:
                    return Conflict(new { mensagem = resultado.Mensagem });

                case TipoResultadoVenda.DadosInvalidos:
                    return BadRequest(new { mensagem = resultado.Mensagem });

                default: // ServicoIndisponivel
                    return StatusCode(StatusCodes.Status503ServiceUnavailable,
                        new { mensagem = resultado.Mensagem });
            }
        }

        // GET /vendas/10
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Venda>> ObterPorId(int id)
        {
            var venda = await _service.ObterPorIdAsync(id);
            if (venda is null) return NotFound(new { mensagem = "Venda não encontrada." });
            return Ok(venda);
        }

        // GET /vendas?passageiroId=1   (sem filtro, lista todas)
        [HttpGet]
        public async Task<ActionResult<List<Venda>>> Listar([FromQuery] long? passageiroId)
        {
            return Ok(await _service.ListarAsync(passageiroId));
        }
    }
}
