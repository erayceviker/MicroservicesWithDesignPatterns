using Shared.Interfaces;

namespace Shared.Events
{
    public record StockReservedEvent(IReadOnlyCollection<OrderItemMessage> OrderItems, Guid CorrelationId) : IStockReservedEvent;
}
