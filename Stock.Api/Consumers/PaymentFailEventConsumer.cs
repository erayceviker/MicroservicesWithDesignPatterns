using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Api.Models;

namespace Stock.Api.Consumers
{
    public class PaymentFailEventConsumer(AppDbContext appDbContext,ILogger<PaymentFailEventConsumer> logger) : IConsumer<PaymentFailEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailEvent> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await appDbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock is not null)
                {
                    stock.Count += item.Count;
                    await appDbContext.SaveChangesAsync();
                }
            }

            logger.LogInformation($"Stock was released for order id : {context.Message.OrderId}");
        }
    }
}
