using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Consumers;
using Order.Api.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(opt =>
{

    opt.AddConsumer<PaymentCompletedEventConsumer>();
    opt.AddConsumer<PaymentFailEventConsumer>();
    opt.AddConsumer<StockNotReservedEventConsumer>();

    opt.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentCompletedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentCompletedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentFailEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderStockNotReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockNotReservedEventConsumer>(ctx);
        });

    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();


app.Run();

