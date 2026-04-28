namespace GoodHamburger.Application.Interfaces;

using GoodHamburger.Domain.Entities;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
}