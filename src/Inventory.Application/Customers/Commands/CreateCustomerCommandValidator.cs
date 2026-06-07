using FluentValidation;
using Inventory.Domain.Enums;
using Inventory.Domain.Utilities;

namespace Inventory.Application.Customers.Commands;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Invalid Name value."); ;

        RuleFor(x => x.Region)
            .NotEmpty()
            .Must(s => EnumStringConverter.TryParseEnum<Region>(s, out _))
            .WithMessage("Invalid Region value.");

    }
}