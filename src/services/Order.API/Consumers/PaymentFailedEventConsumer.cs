using MassTransit;
using Order.API.Contexts;
using Order.API.Enums;
using Shared.Events;

namespace Order.API.Consumers;

public class PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, ApplicationDbContext dbContext) : IConsumer<PaymentFailedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<PaymentFailedEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
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
            _logger.LogError($"Order (Id={context.Message.OrderId}) not found");

    }
}
