using Inventory.Application.Common.Interfaces;
using MediatR;

namespace Inventory.Application.Products.Commands;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _repo;

    public UpdateProductHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return false;

        product.Id = request.Id;
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        await _repo.UpdateAsync(product, cancellationToken);

        return true;
    }
}