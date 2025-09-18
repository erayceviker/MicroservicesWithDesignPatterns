namespace Shared.Interfaces
{
    public interface IOrderRequestFailedEvent 
    {
        int OrderId { get; }
        string FailMessage { get; }
    }
}
