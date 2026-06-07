using Inventory.Application.Common.Dtos;
using MediatR;

namespace Inventory.Application.Customers.Commands;

public record CreateCustomerCommand(
    string Name,
    string Region
) :IRequest<IdResponse>;