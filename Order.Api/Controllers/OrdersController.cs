using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Dtos;
using Order.Api.Models;
using Shared;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(AppDbContext appDbContext,IPublishEndpoint publishEndpoint) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreate)
        {

            var newOrder = new Models.Order
            {
                BuyerId = orderCreate.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address()
                {
                    Line = orderCreate.Address.Line, District = orderCreate.Address.District,
                    Province = orderCreate.Address.Province
                },
                CreatedDate = DateTime.Now
            };

            orderCreate.OrderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem(){Price = item.Price,ProductId = item.ProductId,Count = item.Count});
            });

            await appDbContext.AddAsync(newOrder);
            await appDbContext.SaveChangesAsync();



            var orderCreatedEvent = new OrderCreatedEvent(
                newOrder.Id,
                newOrder.BuyerId,
                new PaymentMessage
                {
                    CardName = orderCreate.Payment.CardName,
                    CardNumber = orderCreate.Payment.CardNumber,
                    Cvv = orderCreate.Payment.Cvv,
                    Exp = orderCreate.Payment.Exp,
                    TotalPrice = orderCreate.OrderItems.Sum(x => x.Price * x.Count)
                },
                orderCreate.OrderItems
                    .Select(item => new OrderItemMessage(){Count = item.Count,ProductId = item.ProductId})
                    .ToList()
            );

            await publishEndpoint.Publish(orderCreatedEvent);


            return Ok();
        }
    }
}
