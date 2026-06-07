using MediatR;

namespace Inventory.Application.Products.Commands;

public record DeactivateProductByIdCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; init; } = id;
}