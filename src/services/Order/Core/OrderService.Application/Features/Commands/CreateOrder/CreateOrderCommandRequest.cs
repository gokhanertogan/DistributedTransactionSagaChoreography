using MediatR;

namespace OrderService.Application.Features.Commands.CreateOrder;

public record CreateOrderCommandRequest : IRequest<CreateOrderCommandResponse>
{
    public int Quantity { get; set; }
    public string? Description { get; set; }
}