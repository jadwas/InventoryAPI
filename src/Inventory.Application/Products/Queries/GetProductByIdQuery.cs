using Inventory.Application.Products.Dtos;
using MediatR;

namespace Inventory.Application.Products.Queries;

public record GetProductByIdQuery(Guid id) : IRequest<ProductDto?>
{
    public Guid Id { get; init; } = id;
}