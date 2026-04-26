namespace GoodHamburger.Application.DTOs;

using GoodHamburger.Domain.Enums;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
}