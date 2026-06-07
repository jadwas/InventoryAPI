using Inventory.Application.Common.Dtos;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using MediatR;

namespace Inventory.Application.Products.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, IdResponse>
{
   
    private readonly IProductRepository _repo;

    public CreateProductHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IdResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            IsActive = true
        };

        await _repo.AddAsync(product, cancellationToken);

        return new IdResponse(product.Id);
    }

}