using Inventory.Api.Extensions;
using Inventory.Api.Middleware;
using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Initialization;
using Serilog;

namespace Inventory.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();

        // Services
        builder.Services
            .AddHttpContextAccessor()
            .AddApiServices(builder.Configuration)
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        var app = builder.Build();

        // Database initialization but not in tests
        if (builder.Configuration["DisableDbInit"] != "true")
        {
            DatabaseInitializer.Initialize(app.Services);
        }


        // Serilog request logging + CID
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diag, httpContext) =>
            {
                var cid = httpContext.RequestServices
                    .GetRequiredService<ICorrelationIdProvider>()
                    .FormattedCorrelationId;

                diag.Set("CID", cid);
            };
        });

        // Middleware pipeline
        app.UseRouting();

        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<EndpointNotFoundMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        // Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapGet("/", () => Results.Redirect("/swagger"));
        }

        app.MapControllers();

        app.Run();
    }
}



//var builder = WebApplication.CreateBuilder(args);

//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .Enrich.FromLogContext()
//    .CreateLogger();

//builder.Host.UseSerilog();
//// Add services
//builder.Services
//    .AddHttpContextAccessor()
//    .AddApiServices(builder.Configuration)
//    .AddApplication()
//    .AddInfrastructure(builder.Configuration)
//    .AddEndpointsApiExplorer()
//    .AddSwaggerGen();

//var app = builder.Build();

////Database initialization 
//DatabaseInitializer.Initialize(app.Services);

//app.UseSerilogRequestLogging(options =>
//{
//    options.EnrichDiagnosticContext = (diag, httpContext) =>
//    {
//        var cid = httpContext.RequestServices
//            .GetRequiredService<ICorrelationIdProvider>()
//            .FormattedCorrelationId;

//        diag.Set("CID", cid);
//    };
//});
//app.UseRouting();

//// Middleware
//app.UseMiddleware<ErrorHandlingMiddleware>();
//app.UseMiddleware<EndpointNotFoundMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

//// Swagger 
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    app.MapGet("/", () => Results.Redirect("/swagger"));
//}

//app.MapControllers();
//app.Run();
