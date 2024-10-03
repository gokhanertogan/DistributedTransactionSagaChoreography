using System.Text.Json;
using MediatR;
using OrderService.Application.Repositories;
using OrderService.Domain.Entities;
using Shared.Events;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommandHandler(IOrderOutboxRepository orderOutboxRepository, IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderOutboxRepository _orderOutboxRepository = orderOutboxRepository;

    public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
    {
        var order = new Order() { Description = request.Description, Quantity = request.Quantity };
        await _orderRepository.AddAsync(order);

        OrderOutbox orderOutbox = new()
        {
            OccurredOn = DateTime.UtcNow,
            ProcessedDate = null,
            Payload = JsonSerializer.Serialize(order),
            Type = nameof(OrderCreatedEvent),
            IdempotentToken = Guid.NewGuid()
        };

        await _orderOutboxRepository.AddAsync(orderOutbox);
        await _orderOutboxRepository.SaveChangesAsync();
        return new();
    }
}
