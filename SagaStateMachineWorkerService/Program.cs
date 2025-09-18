using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachineWorkerService;
using SagaStateMachineWorkerService.Models;
using Shared;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine,OrderStateInstance>().EntityFrameworkRepository(opt =>
    {
        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), m =>
            {
                m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
        });

        cfg.UsingRabbitMq((ctx, configure) =>
        {
            configure.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

            configure.ReceiveEndpoint(RabbitMqSettingsConst.OrderSaga, e =>
            {
                e.ConfigureSaga<OrderStateInstance>(ctx);
            });

        });

    });
});





var host = builder.Build();
host.Run();
