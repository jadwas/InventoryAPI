using FluentValidation;
using Inventory.Application.Common.Interfaces;

namespace Inventory.Application.Products.Commands;

public class DeleteProductByIdValidator : AbstractValidator<DeleteProductByIdCommand>
{
    private readonly IOrderItemRepository _orderItemRepo;

    public DeleteProductByIdValidator(IOrderItemRepository orderItemRepo)
    {
        _orderItemRepo = orderItemRepo;
        
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.Id)
            .MustAsync((s, cancellationToken)=>ProductBeDeleted(s,cancellationToken))
            .WithMessage("Cannot delete product that was used in Order(s).");
    }

    private async Task<bool> ProductBeDeleted(Guid productId,CancellationToken cancellationToken)
    {
        return !await _orderItemRepo.CheckProductInOrders(productId, cancellationToken);
    }
}