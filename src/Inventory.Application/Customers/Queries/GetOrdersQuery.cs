using Inventory.Application.Customers.Dtos;
using MediatR;

namespace Inventory.Application.Customers.Queries;

public record GetCustomersQuery() : IRequest<IEnumerable<CustomerDto>>;