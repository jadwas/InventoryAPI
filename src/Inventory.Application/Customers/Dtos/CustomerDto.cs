using Inventory.Domain.Enums;

namespace Inventory.Application.Customers.Dtos
{
    public record CustomerDto(
        Guid Id,
        string Name,
        Region Region
    );

}
