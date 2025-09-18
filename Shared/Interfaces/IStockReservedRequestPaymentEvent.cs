using MassTransit;

namespace Shared.Interfaces
{
    public interface IStockReservedRequestPaymentEvent : CorrelatedBy<Guid>
    {
        PaymentMessage Payment { get; }
        IReadOnlyCollection<OrderItemMessage> OrderItems { get; }
        string BuyerId { get; }
    }
}
