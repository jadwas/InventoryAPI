namespace Inventory.Application.Customers.Dtos
{
    public record UpdateOrderStatusRequest(Guid OrderId, string Status);


}
