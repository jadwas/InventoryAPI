using Inventory.Application.Common.Interfaces;

namespace Inventory.Infrastructure.Services;

public class DateProvider : IDateProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
}