using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Contexts;

namespace Stock.API.Consumers;

public class PaymentFailedEventConsumer(ILogger<PaymentFailedEventConsumer> logger, ApplicationDbContext dbContext) : IConsumer<PaymentFailedEvent>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<PaymentFailedEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
    {
        foreach (var item in context.Message.OrderItems)
        {
            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

            if (stock != null)
            {
                stock.Count += item.Count;
                await _dbContext.SaveChangesAsync();
            }
        }

        _logger.LogInformation($"Stock was released for order id ({context.Message.OrderId})");
    }
}
