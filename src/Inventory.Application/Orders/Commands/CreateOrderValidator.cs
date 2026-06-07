using FluentValidation;

namespace Inventory.Application.Orders.Commands;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item.");

        RuleFor(x => x.Items)
            .Must(m=>!m.GroupBy(g => g.ProductId, (id, grouped) => grouped.Count() > 1).Any(a => a))
            .WithMessage("Products has to be grouped.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}