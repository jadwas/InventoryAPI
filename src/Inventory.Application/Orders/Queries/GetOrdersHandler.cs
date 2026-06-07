using Inventory.Application.Common.Interfaces;
using Inventory.Application.Customers.Dtos;
using Inventory.Application.Orders.Dtos;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using MediatR;

namespace Inventory.Application.Orders.Queries;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDetailsDto>>
{
    private readonly IOrderRepository _orders;

    public GetOrdersHandler(IOrderRepository orders)
    {
        _orders = orders;
    }
    
    private static CustomerDto MapCustomerToDto(Customer customer)
    {
        return new CustomerDto(customer.Id,customer.Name, Region.Europe);
    }

    public async Task<IEnumerable<OrderDetailsDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);

        return orders.Select(order => new OrderDetailsDto(
            order.Id,
            MapCustomerToDto(order.Customer),
            order.CreatedAt,
            order.UpdatedAt,
            order.Status,
            order.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Product?.Name ?? string.Empty,
                    i.Quantity,
                    i.UnitPrice))
                .ToList()));
    }
    
}