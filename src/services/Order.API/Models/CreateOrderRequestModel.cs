namespace Order.API.Models;

public record OrderCreateRequestModel(
        string BuyerId,
        List<OrderItemRequestModel> OrderItems,
        PaymentRequestModel Payment,
        AddressRequestModel Address);

public record OrderItemRequestModel(
    int ProductId,
    int Count,
    int Price);

public record PaymentRequestModel(
    string CardName,
    string CardNumber,
    string Expiration,
    string CVV,
    decimal TotalPrice);

public record AddressRequestModel(
    string Line,
    string Province,
    string District);
