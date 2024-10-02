using OrderService.Application.Repositories;
using OrderService.Domain.Entities;
using OrderService.Persistence.Contexts;

namespace OrderService.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext dbContext) : Repository<Order>(dbContext), IOrderRepository
{
}
