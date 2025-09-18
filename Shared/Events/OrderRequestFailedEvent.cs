using Shared.Interfaces;

namespace Shared.Events
{
    public record OrderRequestFailedEvent(int OrderId, string FailMessage) : IOrderRequestFailedEvent;
}
