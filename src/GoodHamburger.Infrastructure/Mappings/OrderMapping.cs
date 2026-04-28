namespace GoodHamburger.Infrastructure.Mappings;

using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrderMapping : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Discount).HasPrecision(5, 2).IsRequired();
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}