using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Api.Consumers;
using Stock.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("StockDb"); });

    builder.Services.AddMassTransit(opt =>
{
    opt.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(ctx);
        });


        cfg.ReceiveEndpoint(RabbitMqSettingsConst.StockPaymentFailEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailEventConsumer>(ctx);
        });
    });


    opt.AddConsumer<OrderCreatedEventConsumer>();
    opt.AddConsumer<PaymentFailEventConsumer>();
});

var app = builder.Build();

app.AddSeedDataExt().ContinueWith(x =>
{
    Console.WriteLine(x.IsFaulted ? x.Exception?.Message : "Seed data has been added.");
});


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();


app.Run();
