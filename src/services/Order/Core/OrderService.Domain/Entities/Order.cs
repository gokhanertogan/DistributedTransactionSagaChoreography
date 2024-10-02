namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}