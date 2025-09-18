using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Models;
using Shared.Interfaces;

namespace Order.Api.Consumers
{
    public class OrderRequestFailedEventConsumer(AppDbContext appDbContext, ILogger<OrderRequestFailedEventConsumer> logger) : IConsumer<IOrderRequestFailedEvent>
    {
        public async Task Consume(ConsumeContext<IOrderRequestFailedEvent> context)
        {
            var order = await appDbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

            if (order is not null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.FailMessage;

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
