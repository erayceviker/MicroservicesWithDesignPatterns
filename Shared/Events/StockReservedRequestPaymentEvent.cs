using Shared.Interfaces;

namespace Shared.Events
{
    public record StockReservedRequestPaymentEvent(PaymentMessage Payment,IReadOnlyCollection<OrderItemMessage> OrderItems, Guid CorrelationId,string BuyerId) : IStockReservedRequestPaymentEvent;

}
