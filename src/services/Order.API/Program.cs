using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Contexts;
using Order.API.Models;
using Order.API.Services;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});

builder.Services.AddTransient<IOrderService, OrderService>();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentCompletedEventConsumer>();
    configure.AddConsumer<PaymentFailedEventConsumer>();
    configure.AddConsumer<StockNotReservedEventConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentCompletedEventQueueName,
                                     e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderPaymentFailedEventQueueName,
                                     e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettingsConst.OrderStockNotReservedEventQueueName,
                                     e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (IOrderService orderService, OrderCreateRequestModel requestModel) =>
{
    if (requestModel is null)
    {
        return Results.BadRequest("Request model cannot be null.");
    }

    await orderService.CreateOrderAsync(requestModel);
    return Results.Ok();
});

app.Run();
