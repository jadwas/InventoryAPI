using FluentValidation;

namespace Inventory.Application.Products.Commands;

public class DeactivateProductByIdValidator : AbstractValidator<DeactivateProductByIdCommand>
{
    

    public DeactivateProductByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
    
}