using Inventory.Application.Orders.Dtos;
using MediatR;

namespace Inventory.Application.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsDto?>
{
    public Guid Id { get; init; } = OrderId;
}