using Inventory.Application.Products.Dtos;
using MediatR;

namespace Inventory.Application.Products.Queries;

public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;