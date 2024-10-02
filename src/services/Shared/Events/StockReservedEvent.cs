using Shared.Messages;

namespace Shared.Events;

public record StockReservedEvent(
        int OrderId,
        int BuyerId,
        PaymentMessage PaymentMessage,
        List<OrderItemMessage> OrderItems);