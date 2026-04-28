namespace GoodHamburger.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal Total { get; set; }
}