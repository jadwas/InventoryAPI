using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Orders.Commands;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<UpdateOrderStatusHandler> _logger;
    private readonly ICorrelationIdProvider _cid;

    public UpdateOrderStatusHandler(IOrderRepository orderRepo, ILogger<UpdateOrderStatusHandler> logger, ICorrelationIdProvider cid)
    {
        _orderRepo = orderRepo;
        _logger = logger;
        _cid = cid;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepo.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return false;
        
        if (!IsValidTransition(order.Status, request.Status))
            throw new DomainException($"Invalid status transition: {order.Status} → {request.Status}");
        _logger.LogInformation("Order {OrderId} -  changing status {Status} → {NewStatus} , skipping update {CID}]", order.Id,order.Status,request.Status,_cid.FormattedCorrelationId);
        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepo.UpdateAsync(order, cancellationToken);

        return true;
    }
    private bool IsValidTransition(OrderStatus current, OrderStatus next)
    {
        return (current, next) switch
        {
            (OrderStatus.New, OrderStatus.Processing) => true,
            (OrderStatus.New, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Completed) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            _ => false
        };
    }
}

