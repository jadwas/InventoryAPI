using Inventory.Domain.Enums;
using MediatR;

namespace Inventory.Application.Orders.Commands;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status
) : IRequest<bool>;