namespace SalesApi.Domain.Events
{
    public class CompraCriada
    {
        public Guid VendaId { get; set; }
        public DateTime DataEvento { get; set; } = DateTime.UtcNow;

        public CompraCriada(Guid vendaId)
        {
            VendaId = vendaId;
        }
    }

    public class CompraAlterada
    {
        public Guid VendaId { get; set; }
        public DateTime DataEvento { get; set; } = DateTime.UtcNow;

        public CompraAlterada(Guid vendaId)
        {
            VendaId = vendaId;
        }
    }

    public class CompraCancelada
    {
        public Guid VendaId { get; set; }
        public DateTime DataEvento { get; set; } = DateTime.UtcNow;

        public CompraCancelada(Guid vendaId)
        {
            VendaId = vendaId;
        }
    }

    public class ItemCancelado
    {
        public Guid VendaId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime DataEvento { get; set; } = DateTime.UtcNow;

        public ItemCancelado(Guid vendaId, Guid itemId)
        {
            VendaId = vendaId;
            ItemId = itemId;
        }
    }
}
