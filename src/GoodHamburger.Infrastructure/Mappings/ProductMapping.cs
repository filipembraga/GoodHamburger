namespace GoodHamburger.Infrastructure.Mappings;

using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Price)
            .HasPrecision(10, 2)
            .IsRequired();
        builder.Property(p => p.Category).IsRequired();

        // Seed data — Menu is populated automatically on migration
        builder.HasData(
            new Product { Id = 1, Name = "X Burger",     Price = 5.00m, Category = ProductCategory.Sandwich },
            new Product { Id = 2, Name = "X Egg",        Price = 4.50m, Category = ProductCategory.Sandwich },
            new Product { Id = 3, Name = "X Bacon",      Price = 7.00m, Category = ProductCategory.Sandwich },
            new Product { Id = 4, Name = "French Fries", Price = 2.00m, Category = ProductCategory.Side },
            new Product { Id = 5, Name = "Soft Drink",   Price = 2.50m, Category = ProductCategory.Drink }
        );
    }
}