using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Initialization;

public static class DatabaseInitializer
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");

        var connection = db.Database.GetDbConnection();
        var dataSource = connection.DataSource;

        var fullPath = Path.GetFullPath(dataSource);
        var directory = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(directory))
        {
            logger.LogWarning("Database folder doesn't exist - to be created: {Dir}", directory);
            Directory.CreateDirectory(directory!);
        }

        if (!File.Exists(fullPath))
        {
            logger.LogWarning("Database file doesn't exist - to be created: {Path}", fullPath);
        }

        try
        {
            db.Database.Migrate();
            logger.LogInformation("Migrations applied.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while migrating database.");
            throw;
        }
    }
}
