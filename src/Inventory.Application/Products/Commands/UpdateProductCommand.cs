using MediatR;

namespace Inventory.Application.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock
) : IRequest<bool>;