using SalesApi.Domain.Entities;
using SalesApi.Domain.Events;
using System.Collections.Concurrent;

namespace SalesApi.Data.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ConcurrentDictionary<Guid, Venda> _vendas;
        private readonly ILogger<VendaRepository> _logger;

        public VendaRepository(ILogger<VendaRepository> logger)
        {
            _vendas = new ConcurrentDictionary<Guid, Venda>();
            _logger = logger;
        }

        public Task<Venda> ObterVendaPorIdAsync(Guid id)
        {
            _vendas.TryGetValue(id, out var venda);
            return Task.FromResult(venda);
        }

        public Task<List<Venda>> ListarVendasAsync()
        {
            var vendas = _vendas.Values.ToList();
            return Task.FromResult(vendas);
        }

        public Task AdicionarVendaAsync(Venda venda)
        {
            venda.Id = Guid.NewGuid();
            _vendas.TryAdd(venda.Id, venda);

            // Log do evento CompraCriada
            var evento = new CompraCriada(venda.Id);
            _logger.LogInformation("Evento: CompraCriada, Venda ID: {VendaId}, DataEvento: {DataEvento}", evento.VendaId, evento.DataEvento);

            return Task.CompletedTask;
        }

        public Task AtualizarVendaAsync(Venda venda)
        {
            if (_vendas.ContainsKey(venda.Id))
            {
                _vendas[venda.Id] = venda;

                // Log do evento CompraAlterada
                var evento = new CompraAlterada(venda.Id);
                _logger.LogInformation("Evento: CompraAlterada, Venda ID: {VendaId}, DataEvento: {DataEvento}", evento.VendaId, evento.DataEvento);
            }
            return Task.CompletedTask;
        }

        public Task RemoverVendaAsync(Guid id)
        {
            if (_vendas.TryRemove(id, out var venda))
            {
                // Log do evento CompraCancelada
                var evento = new CompraCancelada(id);
                _logger.LogInformation("Evento: CompraCancelada, Venda ID: {VendaId}, DataEvento: {DataEvento}", evento.VendaId, evento.DataEvento);
            }
            return Task.CompletedTask;
        }

        public Task CancelarItemAsync(Guid vendaId, Guid itemId)
        {
            if (_vendas.TryGetValue(vendaId, out var venda))
            {
                var item = venda.Itens.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    venda.Itens.Remove(item);
                }
            }
            return Task.CompletedTask;
        }

    }
}
