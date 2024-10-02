using OrderService.Application.Repositories;
using OrderService.Domain.Entities;
using OrderService.Persistence.Contexts;

namespace OrderService.Persistence.Repositories;

public class OrderOutboxRepository(ApplicationDbContext dbContext) : Repository<OrderOutbox>(dbContext), IOrderOutboxRepository
{
}