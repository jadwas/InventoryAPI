using Inventory.Application;
using Inventory.Infrastructure;

namespace Inventory.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ApplicationDependencyInjection.AddApplication(services);
        return services;
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        InfrastructureDependencyInjection.AddInfrastructure(services, configuration);
        return services;
    }
}