using FluentAssertions;
using Moq;
using GoodHamburger.API.Controllers;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Tests.API;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _serviceMock;
    private readonly OrderController _controller;

    private static OrderDto MakeOrderDto() => new()
    {
        Id                 = Guid.NewGuid(),
        OrderNumber        = 1,
        CreatedAt          = DateTime.UtcNow,
        Items              = new List<OrderItemDto>(),
        Subtotal           = 5.00m,
        DiscountPercentage = 0,
        Total              = 5.00m
    };

    public OrdersControllerTests()
    {
        _serviceMock = new Mock<IOrderService>();
        _controller  = new OrderController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithOrders()
    {
        var orders = new List<OrderDto> { MakeOrderDto() };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

        var result = await _controller.GetAll() as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var order = MakeOrderDto();
        _serviceMock.Setup(s => s.GetByIdAsync(order.Id)).ReturnsAsync(order);

        var result = await _controller.GetById(order.Id) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new KeyNotFoundException("Order not found."));

        var result = await _controller.GetById(Guid.NewGuid()) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreated()
    {
        var order = MakeOrderDto();
        var dto   = new OrderRequestDto { ProductIds = new List<int> { 1 } };
        _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(order);

        var result = await _controller.Create(dto) as CreatedAtActionResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Create_DuplicateCategory_ReturnsBadRequest()
    {
        var dto = new OrderRequestDto { ProductIds = new List<int> { 1, 2 } };
        _serviceMock.Setup(s => s.CreateAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Duplicate item."));

        var result = await _controller.Create(dto) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Update_ValidRequest_ReturnsOk()
    {
        var order = MakeOrderDto();
        var dto   = new OrderRequestDto { ProductIds = new List<int> { 1 } };
        _serviceMock.Setup(s => s.UpdateAsync(order.Id, dto)).ReturnsAsync(order);

        var result = await _controller.Update(order.Id, dto) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Update_NonExistingId_ReturnsNotFound()
    {
        var dto = new OrderRequestDto { ProductIds = new List<int> { 1 } };
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new KeyNotFoundException("Order not found."));

        var result = await _controller.Update(Guid.NewGuid(), dto) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Update_DuplicateCategory_ReturnsBadRequest()
    {
        var dto = new OrderRequestDto { ProductIds = new List<int> { 1, 2 } };
        _serviceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), dto))
            .ThrowsAsync(new InvalidOperationException("Duplicate item."));

        var result = await _controller.Update(Guid.NewGuid(), dto) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsNoContent()
    {
        var order = MakeOrderDto();
        _serviceMock.Setup(s => s.DeleteAsync(order.Id)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(order.Id) as NoContentResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new KeyNotFoundException("Order not found."));

        var result = await _controller.Delete(Guid.NewGuid()) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
    }
}