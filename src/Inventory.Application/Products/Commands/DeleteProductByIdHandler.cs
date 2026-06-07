using Inventory.Application.Common.Interfaces;
using MediatR;

namespace Inventory.Application.Products.Commands;

public class DeleteProductByIdHandler : IRequestHandler<DeleteProductByIdCommand, bool>
{
    private readonly IProductRepository _productRepo;
    

    public DeleteProductByIdHandler(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<bool> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepo.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return false;
        await _productRepo.DeleteAsync(product, cancellationToken);
        return true;
    }
}