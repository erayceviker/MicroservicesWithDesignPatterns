using Shared.Interfaces;

namespace Shared.Events
{
    public record OrderCreatedRequestEvent(
        int OrderId,
        string BuyerId,
        PaymentMessage Payment,
        IReadOnlyList<OrderItemMessage> OrderItems
    ) : IOrderCreatedRequestEvent;
}
