using FluentValidation;

namespace Inventory.Application.Products.Commands;

public class ActivateProductByIdValidator : AbstractValidator<DeactivateProductByIdCommand>
{
    

    public ActivateProductByIdValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
    
}