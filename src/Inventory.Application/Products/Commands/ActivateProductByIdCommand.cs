using MediatR;

namespace Inventory.Application.Products.Commands;

public record ActivateProductByIdCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; init; } = id;
}