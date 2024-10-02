using OrderService.Domain.Entities;

namespace OrderService.Application.Repositories;

public interface IOrderOutboxRepository : IRepository<OrderOutbox>
{

}