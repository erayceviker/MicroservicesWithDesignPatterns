using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models;
using Shared;

namespace Order.Api.Consumers
{
    public class PaymentCompletedEventConsumer(AppDbContext appDbContext,ILogger<PaymentCompletedEventConsumer> logger) : IConsumer<PaymentCompletedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await appDbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

            if (order is not null)
            {
                order.Status = OrderStatus.Completed;

                await appDbContext.SaveChangesAsync();

                logger.LogInformation($"Order Id : {context.Message.OrderId} status changed : {order.Status}");
            }
            else
            {
                logger.LogInformation($"Order Id : {context.Message.OrderId} not found");
            }
        }
    }
}
