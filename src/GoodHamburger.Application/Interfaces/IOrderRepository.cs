namespace GoodHamburger.Application.Interfaces;

using GoodHamburger.Domain.Entities;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
    Task UpdateAsync(Guid id, List<OrderItem> newItems, decimal discount);
    Task DeleteAsync(Order order);
}