using Microsoft.AspNetCore.Mvc;
using SalesApi.Data.Repositories;
using SalesApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SalesApi.Controllers
{
    [ApiController]
    [Route("api/vendas")]
    public class VendasController : ControllerBase
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly ILogger<VendasController> _logger;

        public VendasController(IVendaRepository vendaRepository, ILogger<VendasController> logger)
        {
            _vendaRepository = vendaRepository;
            _logger = logger;
        }

        // GET: api/vendas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vendas = await _vendaRepository.ListarVendasAsync();
            return Ok(vendas);
        }

        // GET: api/vendas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var venda = await _vendaRepository.ObterVendaPorIdAsync(id);
            if (venda == null)
                return NotFound();

            return Ok(venda);
        }

        // POST: api/vendas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Venda venda)
        {
            if (venda == null)
            {
                return BadRequest("Venda inválida.");
            }

            await _vendaRepository.AdicionarVendaAsync(venda);
            _logger.LogInformation("Venda criada com ID: {VendaId}", venda.Id);

            return CreatedAtAction(nameof(Get), new { id = venda.Id }, venda);
        }

        // PUT: api/vendas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Venda venda)
        {
            if (venda == null || id != venda.Id)
            {
                return BadRequest("ID da venda é inválido.");
            }

            var vendaExistente = await _vendaRepository.ObterVendaPorIdAsync(id);
            if (vendaExistente == null)
            {
                return NotFound();
            }

            await _vendaRepository.AtualizarVendaAsync(venda);
            _logger.LogInformation("Venda atualizada com ID: {VendaId}", venda.Id);

            return NoContent();
        }

        // DELETE: api/vendas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vendaExistente = await _vendaRepository.ObterVendaPorIdAsync(id);
            if (vendaExistente == null)
            {
                return NotFound();
            }

            await _vendaRepository.RemoverVendaAsync(id);
            _logger.LogInformation("Venda removida com ID: {VendaId}", id);

            return NoContent();
        }

        [HttpDelete("{vendaId}/itens/{itemId}")]
        public async Task<IActionResult> CancelarItem(Guid vendaId, Guid itemId)
        {
            await _vendaRepository.CancelarItemAsync(vendaId, itemId);
            return NoContent();
        }
    }
}
