namespace GoodHamburger.Infrastructure.Context;

using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore; 

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