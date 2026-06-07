namespace Inventory.Application.Common.Interfaces;

public interface IOrderItemRepository
{
    Task <bool>CheckProductInOrders(Guid productId, CancellationToken cancellationToken);
    Task <bool>CheckProductsInOrders(Guid[] productsId, CancellationToken cancellationToken);
    
}