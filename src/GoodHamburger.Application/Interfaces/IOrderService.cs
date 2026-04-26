namespace GoodHamburger.Application.Interfaces;

using GoodHamburger.Application.DTOs;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<OrderDto> CreateAsync(OrderRequestDto dto);
    Task<OrderDto> UpdateAsync(Guid id, OrderRequestDto dto);
    Task DeleteAsync(Guid id);
}