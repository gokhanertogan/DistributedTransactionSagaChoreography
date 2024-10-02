using OrderService.Application.Repositories;
using OrderService.Domain.Entities;
using OrderService.Persistence.Contexts;

namespace OrderService.Persistence.Repositories;

public class OrderInboxRepository(ApplicationDbContext dbContext) : Repository<OrderInbox>(dbContext), IOrderInboxRepository
{
}