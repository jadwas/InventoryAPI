using Inventory.Application.Common.Dtos;
using Inventory.Application.Orders.Dtos;
using MediatR;

namespace Inventory.Application.Orders.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemDto> Items
) : IRequest<IdResponse>;