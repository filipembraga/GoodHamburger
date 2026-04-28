using FluentAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Tests.Domain;

public class OrderTests
{
    private static OrderItem MakeItem(decimal price, ProductCategory category) => new()
    {
        Id        = Guid.NewGuid(),
        ProductId = (int)category + 1,
        Product   = new Product
        {
            Id       = (int)category + 1,
            Name     = category.ToString(),
            Price    = price,
            Category = category
        }
    };

    [Fact]
    public void Subtotal_WithNoItems_ReturnsZero()
    {
        var order = new Order();
        order.Subtotal.Should().Be(0.00m);
    }

    [Fact]
    public void Subtotal_WithMultipleItems_ReturnsSumOfPrices()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                MakeItem(5.00m, ProductCategory.Sandwich),
                MakeItem(2.00m, ProductCategory.Side),
                MakeItem(2.50m, ProductCategory.Drink)
            }
        };

        order.Subtotal.Should().Be(9.50m);
    }

    [Fact]
    public void Total_WithNoDiscount_EqualsSubtotal()
    {
        var order = new Order
        {
            Items    = new List<OrderItem> { MakeItem(5.00m, ProductCategory.Sandwich) },
            Discount = 0.00m
        };

        order.Total.Should().Be(order.Subtotal);
    }

    [Fact]
    public void Total_WithTenPercentDiscount_ReturnsCorrectValue()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                MakeItem(5.00m, ProductCategory.Sandwich),
                MakeItem(2.00m, ProductCategory.Side)
            },
            Discount = 0.10m
        };

        order.Total.Should().Be(6.30m);
    }

    [Fact]
    public void Total_WithFifteenPercentDiscount_ReturnsCorrectValue()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                MakeItem(5.00m, ProductCategory.Sandwich),
                MakeItem(2.50m, ProductCategory.Drink)
            },
            Discount = 0.15m
        };

        order.Total.Should().Be(6.375m);
    }

    [Fact]
    public void Total_WithTwentyPercentDiscount_ReturnsCorrectValue()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                MakeItem(5.00m, ProductCategory.Sandwich),
                MakeItem(2.00m, ProductCategory.Side),
                MakeItem(2.50m, ProductCategory.Drink)
            },
            Discount = 0.20m
        };

        order.Total.Should().Be(7.60m);
    }
}