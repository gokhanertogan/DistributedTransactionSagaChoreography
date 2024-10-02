using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Repositories;
using OrderService.Persistence.Contexts;
using OrderService.Persistence.Repositories;

namespace OrderService.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnectionString")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderOutboxRepository, OrderOutboxRepository>();
        services.AddScoped<IOrderInboxRepository, OrderInboxRepository>();
    }
}