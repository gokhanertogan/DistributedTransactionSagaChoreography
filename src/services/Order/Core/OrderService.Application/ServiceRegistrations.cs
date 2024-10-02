using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ServiceRegistrations
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceRegistrations));
    }
}