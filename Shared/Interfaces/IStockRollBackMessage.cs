namespace Shared.Interfaces
{
    public interface IStockRollBackMessage
    {
        IReadOnlyCollection<OrderItemMessage> OrderItems { get; }
        int OrderId { get; }
    }
}
