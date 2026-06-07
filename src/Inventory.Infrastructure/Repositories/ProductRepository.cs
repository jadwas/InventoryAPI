using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductRepository> _logger;
    

    public ProductRepository(AppDbContext db, ILogger<ProductRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Customer ({Name},{Description},{Price})", product.Name, product.Description, product.Price);
        await _db.Products.AddAsync(product, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all products");
        return await _db.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Product?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Product [ID:{Id}]", id);
        return await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(f=>f.Id == id, cancellationToken);
    }
    public async Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing Products [ID:{Id}]", product.Id);
        _db.Products.Remove(product);
        await _db.SaveChangesAsync( cancellationToken);
    }
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating Product [ID:{Id}]", product.Id);
        _db.Products.Update(product);
        await _db.SaveChangesAsync( cancellationToken);
    }
    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Products by list of Ids [{Id}]", string.Join(",", ids));
        return await _db.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating list Products [{Id}]", string.Join(",", products.Select(s=>s.Id)));
        _db.Products.UpdateRange(products);
        await _db.SaveChangesAsync(cancellationToken);
    }
}