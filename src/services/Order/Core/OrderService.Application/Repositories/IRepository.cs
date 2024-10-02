using System.Linq.Expressions;

namespace OrderService.Application.Repositories;

public interface IRepository<T>
{
    IQueryable<T> GetAll();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> method);
    Task AddAsync(T model);
    Task SaveChangesAsync();
}