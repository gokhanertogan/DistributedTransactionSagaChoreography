using MassTransit;
using Order.API.Contexts;
using Shared.Events;

namespace Order.API.Consumers;

public class PaymentCompletedEventConsumer(ILogger<PaymentCompletedEventConsumer> logger, ApplicationDbContext dbContext) : IConsumer<PaymentCompletedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<PaymentCompletedEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);

        if (order is null)
        {
            order!.Status = Enums.OrderStatus.Complete;
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order?.Status}");
        }

        else
            _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
    }
}
