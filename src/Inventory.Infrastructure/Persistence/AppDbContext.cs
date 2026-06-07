using Inventory.Domain.Converters;
using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
        AddEnumConverters(modelBuilder);
    }

    private static void AddEnumConverters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var propertyType = property.ClrType;
                if (propertyType.IsEnum)
                {
                    var converterType = typeof(EnumMemberConverter<>).MakeGenericType(propertyType);
                    var converter = Activator.CreateInstance(converterType);

                    property.SetValueConverter((ValueConverter)converter!);
                }
                if (Nullable.GetUnderlyingType(propertyType) is { IsEnum: true } enumType)
                {
                    var converterType = typeof(EnumMemberConverter<>).MakeGenericType(enumType);
                    var converter = Activator.CreateInstance(converterType);

                    property.SetValueConverter((ValueConverter)converter!);
                }
            }
        }
    }
}