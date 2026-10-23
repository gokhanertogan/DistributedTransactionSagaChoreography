using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using Stock.API.Consumers;
using Stock.API.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockPaymentFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Stocks.Any())
    {
        dbContext.Stocks.AddRange(
            new Stock.API.Entities.Stock { ProductId = 1, Count = 200 },
            new Stock.API.Entities.Stock { ProductId = 2, Count = 100 },
            new Stock.API.Entities.Stock { ProductId = 3, Count = 50 },
            new Stock.API.Entities.Stock { ProductId = 4, Count = 10 });
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();