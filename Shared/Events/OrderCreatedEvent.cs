using Shared.Interfaces;

namespace Shared.Events
{
    public record OrderCreatedEvent(IReadOnlyCollection<OrderItemMessage> OrderItems,Guid CorrelationId) : IOrderCreatedEvent;
}
