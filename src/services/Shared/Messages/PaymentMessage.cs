namespace Shared.Messages;

public record PaymentMessage(
    string CardName,
    string CardNumber,
    string Expiration,
    string CVV,
    decimal TotalPrice);
