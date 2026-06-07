using Inventory.Application.Customers.Dtos;
using Inventory.Domain.Enums;

namespace Inventory.Application.Orders.Dtos
{
    public record OrderDetailsDto(
        Guid Id,
        CustomerDto Customer,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        OrderStatus Status,
        List<OrderItemDto> Items
    );
}
