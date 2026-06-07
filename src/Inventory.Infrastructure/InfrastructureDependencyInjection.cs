using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Configuration;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this  IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = new DatabaseOptions();
        configuration.GetSection("Database").Bind(dbOptions);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(dbOptions.ConnectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDateProvider, DateProvider>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<IDiscountPolicy, DiscountPolicy>();

        return services;
    }
}