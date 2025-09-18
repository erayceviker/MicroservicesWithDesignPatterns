using MassTransit;

namespace Shared.Interfaces
{
    public interface IPaymentFailEvent : CorrelatedBy<Guid>
    {
        string FailMessage { get; }
        IReadOnlyCollection<OrderItemMessage> OrderItems { get; }
    }
}