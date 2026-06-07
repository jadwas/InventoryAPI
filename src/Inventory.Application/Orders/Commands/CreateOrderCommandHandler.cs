using Inventory.Application.Common.Dtos;
using Inventory.Application.Common.Exceptions;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Domain.Exceptions;
using MediatR;

namespace Inventory.Application.Orders.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, IdResponse>
{
    private readonly IProductRepository _productRepo;
    private readonly IOrderRepository _orderRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IPricingService _pricing;
    private readonly IDiscountPolicy _discounts;
    private readonly IDateProvider _date;

    public CreateOrderCommandHandler(
        IProductRepository productRepo,
        IOrderRepository orderRepo,
        ICustomerRepository customerRepo,
        IPricingService pricing,
        IDiscountPolicy discounts,
        IDateProvider date)
    {
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _customerRepo = customerRepo;
        _pricing = pricing;
        _discounts = discounts;
        _date = date;
    }

    public async Task<IdResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Load products
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _productRepo.GetByIdsAsync(productIds, cancellationToken);
        
        if(products.Any(a=>!a.IsActive))
            throw new DomainException("One or more products are inactive.");

        if (products.Count != productIds.Count)
            throw new BadRequestException("One or more products does not exist.");
        
        //Load Customer
        var customer = await _customerRepo.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
            throw new BadRequestException("Customer with given id does not exist.");

        // Validate stock
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
                throw new DomainException($"Not enough stock for product {product.Name}.");
        }

        var creationDate = _date.UtcNow();
        // Create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            CreatedAt = creationDate,
            UpdatedAt = creationDate,
            Status = OrderStatus.New,
            Items = new List<OrderItem>()
        };

        // Build items
        var itemIndex = 0;
        foreach (var item in request.Items.Select(s=>new {Item = s, Product = products.FirstOrDefault(f=>f.Id==s.ProductId)}))
        {
            var discountedPrice = _discounts.CalculateDiscount(itemIndex, item.Product, item.Product.Price, item.Item.Quantity, creationDate)*
                                  _pricing.RegionBasedMultiplier(customer.Region);

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.Item.ProductId,
                Quantity = item.Item.Quantity,
                UnitPrice = discountedPrice
            });

            // Update stock
            item.Product.Stock -= item.Item.Quantity;
            itemIndex++;
        }

        // Save
        await _orderRepo.AddAsync(order, cancellationToken);
        await _productRepo.UpdateRangeAsync(products, cancellationToken);

        return new IdResponse(order.Id);
    }
}

