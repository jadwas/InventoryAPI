using Inventory.Application.Common.Interfaces;
using MediatR;

namespace Inventory.Application.Products.Commands;

public class ActivateProductByIdHandler : IRequestHandler<ActivateProductByIdCommand, bool>
{
    private readonly IProductRepository _repo;

    public ActivateProductByIdHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(ActivateProductByIdCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return false;
        product.IsActive = true;
        await _repo.UpdateAsync(product, cancellationToken);
        return true;
    }
}