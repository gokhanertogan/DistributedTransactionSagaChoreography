using Shared.Messages;

namespace Shared.Events;

public record PaymentFailedEvent(int OrderId,
    int BuyerId,
    string Message,
    List<OrderItemMessage> OrderItems);