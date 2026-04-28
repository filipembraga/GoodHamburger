using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace   GoodHamburger.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
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
        var products = await ResolveProducts(dto.ProductIds);
        ValidateDuplicateCategories(products);

        var order = new Order();

        order.Items    = BuildItems(products, order.Id);
        order.Discount = DiscountCalculator.CalculateDiscount(order.Items);

        await _orderRepository.AddAsync(order);
        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateAsync(Guid id, OrderRequestDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order {id} not found.");

        var products = await ResolveProducts(dto.ProductIds);
        ValidateDuplicateCategories(products);

        var newItems = BuildItems(products, id);
        var discount = DiscountCalculator.CalculateDiscount(newItems);

        await _orderRepository.UpdateAsync(id, newItems, discount);
        return await GetByIdAsync(id);
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
        OrderNumber        = order.OrderNumber,
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

    private async Task<List<Product>> ResolveProducts(List<int> productIds)
    {
        var products = new List<Product>();

        foreach (var id in productIds)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Product {id} not found in menu.");

            products.Add(product);
        }

        return products;
    }

    private static void ValidateDuplicateCategories(List<Product> products)
    {
        var duplicate = products
            .GroupBy(p => p.Category)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate is not null)
            throw new InvalidOperationException(
                $"Duplicate item: only one {duplicate.Key} is allowed per order.");
    }

    private static List<OrderItem> BuildItems(List<Product> products, Guid orderId)
    {
        return products.Select(p => new OrderItem
        {
            Id        = Guid.NewGuid(),
            ProductId = p.Id,
            Product   = p,
            OrderId   = orderId
        }).ToList();
    }
    #endregion

}