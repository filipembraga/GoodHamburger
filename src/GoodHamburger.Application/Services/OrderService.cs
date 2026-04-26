using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace   GoodHamburger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    private static readonly List<Product> _menu = new()
    {
        new Product { Id = 1, Name = "X Burger",     Price = 5.00m, Category = ProductCategory.Sandwich },
        new Product { Id = 2, Name = "X Egg",        Price = 4.50m, Category = ProductCategory.Sandwich },
        new Product { Id = 3, Name = "X Bacon",      Price = 7.00m, Category = ProductCategory.Sandwich },
        new Product { Id = 4, Name = "French Fries", Price = 2.00m, Category = ProductCategory.Side },
        new Product { Id = 5, Name = "Soft Drink",   Price = 2.50m, Category = ProductCategory.Drink },
    };

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order {id} not found.");

        return MapToDto(order);
    }

    public async Task<OrderDto> CreateAsync(OrderRequestDto dto)
    {
        var products = ResolveProducts(dto.ProductIds);
        ValidateDuplicateCategories(products);

        var order = new Order();
        order.Items = products.Select(p => new OrderItem
        {
            ProductId = p.Id,
            Product = p,
            OrderId = order.Id
        }).ToList();  

        order.Discount = DiscountCalculator.CalculateDiscount(order.Items); 

        await _orderRepository.AddAsync(order);
        return MapToDto(order);  
    }

    public async Task<OrderDto> UpdateAsync(Guid id, OrderRequestDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order {id} not found.");

        var products = ResolveProducts(dto.ProductIds);
        ValidateDuplicateCategories(products);

        order.Items = products.Select(p => new OrderItem
        {
            ProductId = p.Id,
            Product = p,
            OrderId = order.Id
        }).ToList();

        order.Discount = DiscountCalculator.CalculateDiscount(order.Items);

        await _orderRepository.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order {id} not found.");
       
        await _orderRepository.DeleteAsync(order);
    }

    #region [Private Helpers]
    private static OrderDto MapToDto(Order order) => new()
    {
        Id                 = order.Id,
        CreatedAt          = order.OrderDate,
        Subtotal           = order.Subtotal,
        DiscountPercentage = order.Discount * 100,
        Total              = order.Total,
        Items              = order.Items.Select(i => new OrderItemDto
        {
            ProductId    = i.ProductId,
            ProductName  = i.Product.Name,
            ProductPrice = i.Product.Price
        }).ToList()
    };

    private static List<Product> ResolveProducts(List<int> productIds)
    {
        var products = new List<Product>();
    
        foreach (var id in productIds)
        {
            var product = _menu.FirstOrDefault(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Product {id} not found in Menu.");
            
            products.Add(product);
        }
        return products;
    }

    private static void ValidateDuplicateCategories(List<Product> products)
    {
        var duplicate = products.GroupBy(p => p.Category)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate != null)
            throw new InvalidOperationException($"Duplicate item: only one {duplicate.Key} is allowed per order.");
    }
    #endregion

}