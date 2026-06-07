using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Enums;

namespace Inventory.Infrastructure.Services;

public class PricingService : IPricingService
{
   

    public decimal RegionBasedMultiplier(Region region)
    {
        return region switch
        {
            Region.US => 1.0m,
            Region.Europe => 1.15m,
            Region.Asia => 1.05m,
            _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
        };
        
    }
}