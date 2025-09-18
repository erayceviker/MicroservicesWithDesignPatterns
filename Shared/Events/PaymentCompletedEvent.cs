using Shared.Interfaces;

namespace Shared.Events
{
    public record PaymentCompletedEvent(Guid CorrelationId) : IPaymentCompletedEvent;

}
