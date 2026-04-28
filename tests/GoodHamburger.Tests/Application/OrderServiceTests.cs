using FluentAssertions;
using Moq;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly OrderService _service;

    // --- Helpers ---

    private static Product MakeProduct(int id, ProductCategory category) => new()
    {
        Id       = id,
        Name     = category.ToString(),
        Price    = 5.00m,
        Category = category
    };

    private static Order MakeOrder(params Product[] products)
    {
        var order = new Order();
        order.Items = products.Select(p => new OrderItem
        {
            Id        = Guid.NewGuid(),
            OrderId   = order.Id,
            ProductId = p.Id,
            Product   = p
        }).ToList();
        return order;
    }

    public OrderServiceTests()
    {
        _orderRepoMock   = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _service         = new OrderService(_orderRepoMock.Object, _productRepoMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var orders = new List<Order> { MakeOrder(MakeProduct(1, ProductCategory.Sandwich)) };
        _orderRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsOrder()
    {
        var order = MakeOrder(MakeProduct(1, ProductCategory.Sandwich));
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _service.GetByIdAsync(order.Id);

        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var act = async () => await _service.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_ReturnsOrderDto()
    {
        var sandwich = MakeProduct(1, ProductCategory.Sandwich);
        var side     = MakeProduct(4, ProductCategory.Side);

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sandwich);
        _productRepoMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(side);
        _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        var dto    = new OrderRequestDto { ProductIds = new List<int> { 1, 4 } };
        var result = await _service.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.DiscountPercentage.Should().Be(10);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCategory_ThrowsInvalidOperationException()
    {
        var sandwich1 = MakeProduct(1, ProductCategory.Sandwich);
        var sandwich2 = MakeProduct(2, ProductCategory.Sandwich);

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sandwich1);
        _productRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(sandwich2);

        var dto = new OrderRequestDto { ProductIds = new List<int> { 1, 2 } };
        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Sandwich*");
    }

    [Fact]
    public async Task CreateAsync_InvalidProductId_ThrowsKeyNotFoundException()
    {
        _productRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var dto = new OrderRequestDto { ProductIds = new List<int> { 99 } };
        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task UpdateAsync_ValidOrder_ReturnsUpdatedDto()
    {
        var sandwich = MakeProduct(1, ProductCategory.Sandwich);
        var drink    = MakeProduct(5, ProductCategory.Drink);

        var existingOrder = MakeOrder(sandwich);

        var updatedOrder = MakeOrder(sandwich, drink);
        updatedOrder.Discount = 0.15m;

        _orderRepoMock
            .SetupSequence(r => r.GetByIdAsync(existingOrder.Id))
            .ReturnsAsync(existingOrder)
            .ReturnsAsync(updatedOrder);

        _productRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(sandwich);
        _productRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(drink);
        _orderRepoMock
            .Setup(r => r.UpdateAsync(existingOrder.Id, It.IsAny<List<OrderItem>>(), It.IsAny<decimal>()))
            .Returns(Task.CompletedTask);

        var dto    = new OrderRequestDto { ProductIds = new List<int> { 1, 5 } };
        var result = await _service.UpdateAsync(existingOrder.Id, dto);

        result.DiscountPercentage.Should().Be(15);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var dto = new OrderRequestDto { ProductIds = new List<int> { 1 } };
        var act = async () => await _service.UpdateAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_CallsRepository()
    {
        var order = MakeOrder(MakeProduct(1, ProductCategory.Sandwich));
        _orderRepoMock.Setup(r => r.GetByIdAsync(order.Id)).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.DeleteAsync(order)).Returns(Task.CompletedTask);

        await _service.DeleteAsync(order.Id);

        _orderRepoMock.Verify(r => r.DeleteAsync(order), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order?)null);

        var act = async () => await _service.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}