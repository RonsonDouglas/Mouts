using SalesApi.Domain.Entities;

namespace SalesApi.Data.Repositories
{
    public interface IVendaRepository
    {
        Task<Venda> ObterVendaPorIdAsync(Guid id);
        Task<List<Venda>> ListarVendasAsync();
        Task AdicionarVendaAsync(Venda venda);
        Task AtualizarVendaAsync(Venda venda);
        Task RemoverVendaAsync(Guid id);
        Task CancelarItemAsync(Guid vendaId, Guid itemId);
    }
}
