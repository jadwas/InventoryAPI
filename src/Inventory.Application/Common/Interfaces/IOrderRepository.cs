using Inventory.Domain.Entities;

namespace Inventory.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken);
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    
}