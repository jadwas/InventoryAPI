using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(AppDbContext db,ILogger<CustomerRepository> logger)
    {
        _logger = logger;
        _db = db;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Customer ({Name},{Region})", customer.Name, customer.Region);
        await _db.Customers.AddAsync(customer, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all customers");
        return await _db.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Customer [ID:{Id}]", id);
        return await _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}