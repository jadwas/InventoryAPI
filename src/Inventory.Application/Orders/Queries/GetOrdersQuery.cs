using Inventory.Application.Orders.Dtos;
using MediatR;

namespace Inventory.Application.Orders.Queries;

public record GetOrdersQuery() : IRequest<IEnumerable<OrderDetailsDto>>;