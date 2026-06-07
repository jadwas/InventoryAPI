using Inventory.Domain.Enums;

namespace Inventory.Application.Common.Interfaces;

public interface IPricingService
{
    decimal RegionBasedMultiplier(Region region);
}