using MassTransit;

namespace Shared.Interfaces
{
    public interface IOrderCreatedEvent : CorrelatedBy<Guid>
    {
        IReadOnlyCollection<OrderItemMessage> OrderItems { get; }
    };

}
