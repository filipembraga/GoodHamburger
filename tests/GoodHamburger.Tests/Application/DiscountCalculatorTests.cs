using FluentAssertions;
using GoodHamburger.Application.Services;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Application;

public class DiscountCalculatorTests
{
    private static OrderItem MakeItem(ProductCategory category) => new()
    {
        Id        = Guid.NewGuid(),
        ProductId = (int)category + 1,
        Product   = new Product
        {
            Id       = (int)category + 1,
            Name     = category.ToString(),
            Price    = 5.00m,
            Category = category
        }
    };

    private static List<OrderItem> Items(params ProductCategory[] categories)
        => categories.Select(MakeItem).ToList();


    // No discount
    [Fact]
    public void Calculate_SandwichOnly_ReturnsNoDiscount()
    {
        var items = Items(ProductCategory.Sandwich);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.00m);
    }

    [Fact]
    public void Calculate_SideOnly_ReturnsNoDiscount()
    {
        var items = Items(ProductCategory.Side);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.00m);
    }

    [Fact]
    public void Calculate_DrinkOnly_ReturnsNoDiscount()
    {
        var items = Items(ProductCategory.Drink);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.00m);
    }

    [Fact]
    public void Calculate_SideAndDrink_ReturnsNoDiscount()
    {
        var items = Items(ProductCategory.Side, ProductCategory.Drink);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.00m);
    }

    // 10% discount
    [Fact]
    public void Calculate_SandwichAndSide_ReturnsTenPercent()
    {
        var items = Items(ProductCategory.Sandwich, ProductCategory.Side);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.10m);
    }

    // 15% discount
    [Fact]
    public void Calculate_SandwichAndDrink_ReturnsFifteenPercent()
    {
        var items = Items(ProductCategory.Sandwich, ProductCategory.Drink);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.15m);
    }

    // 20% discount
    [Fact]
    public void Calculate_SandwichSideAndDrink_ReturnsTwentyPercent()
    {
        var items = Items(ProductCategory.Sandwich, ProductCategory.Side, ProductCategory.Drink);
        var result = DiscountCalculator.CalculateDiscount(items);
        result.Should().Be(0.20m);
    }

    // Empty

    [Fact]
    public void Calculate_EmptyList_ReturnsNoDiscount()
    {
        var result = DiscountCalculator.CalculateDiscount(new List<OrderItem>());
        result.Should().Be(0.00m);
    }
}