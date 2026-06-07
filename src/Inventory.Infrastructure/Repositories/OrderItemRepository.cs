using Inventory.Application.Common.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrderItemRepository> _logger;
    

    public OrderItemRepository(AppDbContext db, ILogger<OrderItemRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> CheckProductInOrders(Guid productId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking existence of products in Orders by id [{Id}]", productId);
        return await _db.OrderItems
            .AnyAsync(p => p.ProductId == productId, cancellationToken: cancellationToken);
    }

    public async Task<bool> CheckProductsInOrders(Guid[] productsId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking existence of products in Orders by list of Ids [{Id}]", string.Join(",", productsId));
        return await _db.OrderItems
            .AnyAsync(p => productsId.Contains(p.ProductId), cancellationToken: cancellationToken);
    }
}