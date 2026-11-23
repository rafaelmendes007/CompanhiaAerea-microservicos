using Microsoft.AspNetCore.Mvc;
using Voos.Api.DTOs;
using Voos.Api.Models;
using Voos.Api.Services;

namespace Voos.Api.Controllers
{
    [ApiController]
    [Route("voos")]
    public class VoosController : ControllerBase
    {
        private readonly IVooService _service;

        public VoosController(IVooService service)
        {
            _service = service;
        }

        // GET /voos?origem=FOR&destino=GRU&data=2026-12-15
        [HttpGet]
        public async Task<ActionResult<List<Voo>>> Listar(
            [FromQuery] string? origem, [FromQuery] string? destino, [FromQuery] DateOnly? data)
        {
            return Ok(await _service.ListarAsync(origem, destino, data));
        }

        // GET /voos/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Voo>> ObterPorId(int id)
        {
            var voo = await _service.ObterPorIdAsync(id);
            if (voo is null) return NotFound(new { mensagem = "Voo não encontrado." });
            return Ok(voo);
        }

        // POST /voos
        [HttpPost]
        public async Task<ActionResult<Voo>> Criar(CriarVooDto dto)
        {
            try
            {
                var voo = await _service.CriarAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = voo.Id }, voo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // POST /voos/1/reservar-assento  (chamada pelo serviço de Vendas)
        [HttpPost("{id:int}/reservar-assento")]
        public async Task<ActionResult<Voo>> ReservarAssento(int id)
        {
            var resultado = await _service.ReservarAssentoAsync(id);
            if (resultado == ResultadoOperacao.NaoEncontrado)
                return NotFound(new { mensagem = "Voo não encontrado." });
            if (resultado == ResultadoOperacao.Conflito)
                return Conflict(new { mensagem = "Não há assentos disponíveis neste voo." });
            return Ok(await _service.ObterPorIdAsync(id));
        }

        // POST /voos/1/liberar-assento  (Vendas desfaz a reserva se o pagamento falhar)
        [HttpPost("{id:int}/liberar-assento")]
        public async Task<ActionResult<Voo>> LiberarAssento(int id)
        {
            var resultado = await _service.LiberarAssentoAsync(id);
            if (resultado == ResultadoOperacao.NaoEncontrado)
                return NotFound(new { mensagem = "Voo não encontrado." });
            if (resultado == ResultadoOperacao.Conflito)
                return Conflict(new { mensagem = "Todos os assentos já estão livres." });
            return Ok(await _service.ObterPorIdAsync(id));
        }
    }
}
