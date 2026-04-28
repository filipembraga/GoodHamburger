using FluentAssertions;
using Moq;
using GoodHamburger.API.Controllers;
using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Tests.API;

public class MenuControllerTests
{
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly MenuController _controller;

    private static Product MakeProduct(int id, ProductCategory category) => new()
    {
        Id       = id,
        Name     = category.ToString(),
        Price    = 5.00m,
        Category = category
    };

    public MenuControllerTests()
    {
        _productRepoMock = new Mock<IProductRepository>();
        _controller      = new MenuController(_productRepoMock.Object);
    }

    [Fact]
    public async Task GetMenu_ReturnsOkWithAllProducts()
    {
        var products = new List<Product>
        {
            MakeProduct(1, ProductCategory.Sandwich),
            MakeProduct(2, ProductCategory.Sandwich),
            MakeProduct(3, ProductCategory.Sandwich),
            MakeProduct(4, ProductCategory.Side),
            MakeProduct(5, ProductCategory.Drink)
        };

        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result = await _controller.GetMenu() as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetMenu_ReturnsAllFiveProducts()
    {
        var products = new List<Product>
        {
            MakeProduct(1, ProductCategory.Sandwich),
            MakeProduct(2, ProductCategory.Sandwich),
            MakeProduct(3, ProductCategory.Sandwich),
            MakeProduct(4, ProductCategory.Side),
            MakeProduct(5, ProductCategory.Drink)
        };

        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var result    = await _controller.GetMenu() as OkObjectResult;
        var returned  = result!.Value as IEnumerable<Product>;

        returned.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetMenu_EmptyRepository_ReturnsOkWithEmptyList()
    {
        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        var result   = await _controller.GetMenu() as OkObjectResult;
        var returned = result!.Value as IEnumerable<Product>;

        result.StatusCode.Should().Be(200);
        returned.Should().BeEmpty();
    }
}