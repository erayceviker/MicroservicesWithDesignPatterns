using MassTransit;

namespace Shared.Interfaces
{
    public interface IStockReservedEvent : CorrelatedBy<Guid>
    {
        public IReadOnlyCollection<OrderItemMessage> OrderItems { get; }
    }
}
