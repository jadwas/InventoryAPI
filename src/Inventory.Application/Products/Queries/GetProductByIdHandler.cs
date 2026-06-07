using Inventory.Application.Common.Interfaces;
using Inventory.Application.Products.Dtos;
using MediatR;

namespace Inventory.Application.Products.Queries;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repo;

    public GetProductByIdHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto? > Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repo.GetByIdAsync(request.Id, cancellationToken);

        return product != null?  new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock
        ): null;
    }
}