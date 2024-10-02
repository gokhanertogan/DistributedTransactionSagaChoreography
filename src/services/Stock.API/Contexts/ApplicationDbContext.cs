using Microsoft.EntityFrameworkCore;

namespace Stock.API.Contexts;
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Entities.Stock> Stocks { get; set; }
}