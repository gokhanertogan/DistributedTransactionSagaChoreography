using MassTransit;
using Order.API.Contexts;
using Order.API.Entities;
using Order.API.Enums;
using Order.API.Models;
using Shared.Events;
using Shared.Messages;

namespace Order.API.Services;

public interface IOrderService
{
    Task CreateOrderAsync(OrderCreateRequestModel requestModel);
}

public class OrderService(IPublishEndpoint publishEndpoint, ApplicationDbContext dbContext) : IOrderService
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task CreateOrderAsync(OrderCreateRequestModel requestModel)
    {
        var newOrder = new Entities.Order()
        {
            BuyerId = requestModel.BuyerId,
            Status = OrderStatus.Suspend,
            Address = new Address { Line = requestModel.Address.Line, District = requestModel.Address.District, Province = requestModel.Address.Province },
            CreatedDate = DateTime.Now
        };

        requestModel.OrderItems.ForEach(item =>
        {
            newOrder.Items.Add(new OrderItem() { Price = item.Price, ProductId = item.ProductId, Count = item.Count });
        });

        await _dbContext.AddAsync(newOrder);
        await _dbContext.SaveChangesAsync();

        var orderCreatedEvent = new OrderCreatedEvent
        (
            BuyerId: int.Parse(requestModel.BuyerId),
            OrderId: newOrder.Id,
            PaymentMessage: new PaymentMessage(
                CardName: requestModel.Payment.CardName,
                CardNumber: requestModel.Payment.CardNumber,
                Expiration: requestModel.Payment.Expiration,
                CVV: requestModel.Payment.CVV,
                TotalPrice: requestModel.OrderItems.Sum(x => x.Price * x.Count)
            ),
            OrderItems: requestModel.OrderItems.Select(x => new OrderItemMessage(x.ProductId, x.Count)).ToList()
        );

        await _publishEndpoint.Publish(orderCreatedEvent);
    }
}
