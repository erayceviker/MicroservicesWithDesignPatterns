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
    opt.AddConsumer<OrderRequestCompletedEventConsumer>();

    opt.AddConsumer<OrderRequestFailedEventConsumer>();


    opt.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderRequestCompletedQueueName, e =>
        {
            e.ConfigureConsumer<OrderRequestCompletedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMqSettingsConst.OrderRequestFailedQueueName, e =>
        {
            e.ConfigureConsumer<OrderRequestFailedEventConsumer>(ctx);
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

