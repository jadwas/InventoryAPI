using MediatR;

namespace Inventory.Application.Products.Commands;

public record DeleteProductByIdCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; init; } = id;
}