namespace Shared.Interfaces
{
    public interface IOrderCreatedRequestEvent
    {
        int OrderId { get; }
        string BuyerId { get; }
        IReadOnlyList<OrderItemMessage> OrderItems { get; }
        PaymentMessage Payment { get; }
    }
}
