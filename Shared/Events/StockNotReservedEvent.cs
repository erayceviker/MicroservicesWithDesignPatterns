using Shared.Interfaces;

namespace Shared.Events
{
    public record StockNotReservedEvent(string FailMessage,Guid CorrelationId) : IStockNotReservedEvent;
}
