using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Services;

public static class DiscountCalculator
{
    public static decimal CalculateDiscount(IEnumerable<OrderItem> items)
    {
        var categories = items.Select(i => i.Product.Category)
        .ToHashSet();

        bool hasSandwich = categories.Contains(ProductCategory.Sandwich);
        bool hasSide = categories.Contains(ProductCategory.Side);
        bool hasDrink = categories.Contains(ProductCategory.Drink);

        if (hasSandwich && hasSide && hasDrink)
        {
            return 0.20m; // 20% discount
        }
        else if (hasSandwich && hasDrink)
        {
            return 0.15m; // 15% discount
        }
        else if (hasSandwich && hasSide)
        {
            return 0.10m; // 10% discount
        }
        return 0m; // No discount
    }
}