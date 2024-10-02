namespace Shared.Events;

public record PaymentCompletedEvent(int OrderId, int BuyerId);