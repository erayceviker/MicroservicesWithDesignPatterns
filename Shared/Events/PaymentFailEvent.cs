using Shared.Interfaces;

namespace Shared.Events
{
    public record PaymentFailEvent(Guid CorrelationId, string BuyerId,IReadOnlyCollection<OrderItemMessage> OrderItems, string FailMessage) : IPaymentFailEvent;
}
