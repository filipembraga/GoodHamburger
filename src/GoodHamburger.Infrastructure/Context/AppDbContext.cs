namespace GoodHamburger.Infrastructure.Context;

using System.Diagnostics.CodeAnalysis;
using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore; 

[ExcludeFromCodeCoverage]
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Order> Orders {get; set; }
    public DbSet<OrderItem> OrderItems {get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);    
    }
}