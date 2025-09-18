using Shared.Interfaces;

namespace Shared.Events
{
    public record OrderRequestCompletedEvent(int OrderId) : IOrderRequestCompletedEvent;

}
