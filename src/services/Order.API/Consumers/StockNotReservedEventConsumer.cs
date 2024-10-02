using MassTransit;
using Order.API.Contexts;
using Order.API.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class StockNotReservedEventConsumer(ApplicationDbContext dbContext, ILogger<PaymentCompletedEventConsumer> logger) : IConsumer<StockNotReservedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<PaymentCompletedEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
        if (order != null)
        {
            order.Status = OrderStatus.Fail;
            order.FailMessage = context.Message.Message;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.Status}");
        }

        else
        {
            _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
        }
    }
}
