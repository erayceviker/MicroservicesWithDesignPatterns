namespace Shared
{
    public record PaymentFailEvent(int OrderId, string BuyerId,List<OrderItemMessage> OrderItems, string FailMessage);

}
