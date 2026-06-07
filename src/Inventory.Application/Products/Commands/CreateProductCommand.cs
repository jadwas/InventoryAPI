using Inventory.Application.Common.Dtos;
using MediatR;

namespace Inventory.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock
) : IRequest<IdResponse>;