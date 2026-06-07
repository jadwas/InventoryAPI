using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerRepository> _logger;

    public OrderRepository(AppDbContext db, ILogger<CustomerRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Order [Customer: {Customer},No of products: {NoOfProducts})", order.Customer?.Name, order.Items.Count);
        await _db.Orders.AddAsync(order, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
  
    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all orders");
        return await _db.Orders
            .Include(i=>i.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Order [ID:{Id}]", id);
        return await _db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Order with items [ID:{Id}]", id);
        return await _db.Orders
            .Include(i => i.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating Order [ID:{Id}]", order.Id);
        _db.Orders.Update(order);
        await _db.SaveChangesAsync(cancellationToken);
    }
}