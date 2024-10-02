using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Repositories;
using OrderService.Persistence.Contexts;

namespace OrderService.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    public Repository(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public DbSet<T> Table { get => _dbContext.Set<T>(); }

    public async Task AddAsync(T model)
          => await Table.AddAsync(model);

    public IQueryable<T> GetAll()
        => Table;

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> method)
        => Table.Where(method);

    public async Task SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();
}
