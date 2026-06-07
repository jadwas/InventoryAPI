using Inventory.Domain.Entities;

namespace Inventory.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}