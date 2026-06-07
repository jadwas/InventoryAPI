namespace Inventory.Application.Common.Interfaces;

public interface IDateProvider
{
    DateTime UtcNow();
}