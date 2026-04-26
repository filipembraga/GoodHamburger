namespace GoodHamburger.Application.DTOs;

public class OrderRequestDto
{
    public List<int> ProductIds { get; set; } = new();
}