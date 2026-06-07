using Inventory.Application.Common.Interfaces;
using MediatR;

namespace Inventory.Application.Products.Commands;

public class DeactivateProductByIdHandler : IRequestHandler<DeactivateProductByIdCommand, bool>
{
    private readonly IProductRepository _repo;

    public DeactivateProductByIdHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeactivateProductByIdCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return false;
        product.IsActive = false;
        await _repo.UpdateAsync(product, cancellationToken);
        return true;
    }
}