using FluentValidation;

namespace Inventory.Application.Products.Commands;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Invalid Name value.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Invalid Description value.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price value cannot be less than 0."); ;

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock value cannot be less than 0."); ;
    }
}