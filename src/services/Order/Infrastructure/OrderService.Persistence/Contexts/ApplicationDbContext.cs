using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Persistence.EntityConfigurations;

namespace OrderService.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderOutbox> OrderOutboxes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderOutboxConfiguration());
    }
}