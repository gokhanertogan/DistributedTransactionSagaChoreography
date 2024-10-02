using Shared.Messages;

namespace Shared.Events;

public record OrderCreatedEvent(
    int OrderId,
    int BuyerId,
    PaymentMessage PaymentMessage,
    List<OrderItemMessage> OrderItems);
