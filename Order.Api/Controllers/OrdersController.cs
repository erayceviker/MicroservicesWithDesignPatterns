using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Dtos;
using Order.Api.Models;
using Shared;
using Shared.Events;
using Shared.Interfaces;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(AppDbContext appDbContext,ISendEndpointProvider sendEndpointProvider) : ControllerBase
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



            var orderCreatedRequestEvent = new OrderCreatedRequestEvent(
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

            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettingsConst.OrderSaga}"));

            await sendEndpoint.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);

            return Ok();
        }
    }
}
