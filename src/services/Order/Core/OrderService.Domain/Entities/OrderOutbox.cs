using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class OrderOutbox
{
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? Type { get; set; }
    public string? Payload { get; set; }
    public Guid IdempotentToken { get; set; }
    public OrderOutboxStatus Status { get; set; }
    public Guid? ProcessedBy { get; set; }
    public int Version { get; set; }
}

