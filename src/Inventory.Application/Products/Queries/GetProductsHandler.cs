using Inventory.Application.Common.Interfaces;
using Inventory.Application.Products.Dtos;
using MediatR;

namespace Inventory.Application.Products.Queries;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _repo;

    public GetProductsHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repo.GetAllAsync(cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock
        ));
    }
}