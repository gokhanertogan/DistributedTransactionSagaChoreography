using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Persistence.EntityConfigurations;

namespace OrderService.Persistence.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ChoreographyOutboxOrderDB;User ID=SA;Password=Gkn12345678*;TrustServerCertificate=True;");
    }


    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderOutbox> OrderOutboxes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderOutboxConfiguration());
    }
}