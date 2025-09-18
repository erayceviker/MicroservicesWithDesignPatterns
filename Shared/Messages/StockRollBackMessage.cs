using Shared.Interfaces;

namespace Shared.Messages
{
    public record StockRollBackMessage(IReadOnlyCollection<OrderItemMessage> OrderItems,int OrderId) : IStockRollBackMessage;

}
