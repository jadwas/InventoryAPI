
using Inventory.Application.Common.Interfaces;
using Inventory.Application.Customers.Dtos;
using Inventory.Application.Orders.Dtos;
using Inventory.Domain.Enums;
using MediatR;

namespace Inventory.Application.Orders.Queries;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
{
    private readonly IOrderRepository _orders;

    public GetOrderByIdHandler(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdWithItemsAsync(request.Id, cancellationToken);

        if (order?.Customer is null)
            return null;
        
        var customer = new CustomerDto(order.Customer.Id, order.Customer.Name, Region.Europe);
        
        var items = order.Items
            .Select(i => new OrderItemDto(
                i.ProductId,
                i.Product?.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice))
            .ToList();

        return new OrderDetailsDto(
            order.Id,
            customer,
            order.CreatedAt,
            order.UpdatedAt,
            order.Status,
            items);
    }
}