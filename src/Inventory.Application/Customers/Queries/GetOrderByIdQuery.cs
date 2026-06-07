using Inventory.Application.Customers.Dtos;
using MediatR;

namespace Inventory.Application.Customers.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>
{
    public Guid Id { get; init; } = CustomerId;
}