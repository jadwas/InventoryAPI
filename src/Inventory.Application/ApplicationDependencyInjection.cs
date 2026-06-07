using FluentValidation;
using Inventory.Application.Common.Behaviors;
using Inventory.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Inventory.Application.Common.Providers;


namespace Inventory.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();

        return services;
    }
}