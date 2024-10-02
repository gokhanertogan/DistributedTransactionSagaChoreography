using MassTransit;
using Shared.Events;

namespace Payment.API.Consumers;

public class StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint endpoint) : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedEventConsumer> _logger = logger;
    private readonly IPublishEndpoint _endpoint = endpoint;

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var balance = 3000m;

        if (balance > context.Message.PaymentMessage.TotalPrice)
        {
            _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice} TL was withdrawn from credit card for user id={context.Message.BuyerId}");
            await _endpoint.Publish(new PaymentCompletedEvent(context.Message.OrderId, context.Message.BuyerId));
        }

        else
        {
            _logger.LogInformation($"{context.Message.PaymentMessage.TotalPrice} TL was not withdrawn from credit card for user id= {context.Message.BuyerId}");

            await _endpoint.Publish(new PaymentFailedEvent(
                        OrderId: context.Message.OrderId,
                        BuyerId: context.Message.BuyerId,
                        Message: "not enough balance",
                        OrderItems: context.Message.OrderItems
                        ));

        }
    }
}
