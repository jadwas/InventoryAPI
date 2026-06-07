using FluentValidation;
using Inventory.Application.Customers.Dtos;
using Inventory.Domain.Enums;
using Inventory.Domain.Utilities;

namespace Inventory.Application.Orders.Commands;

public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s=> EnumStringConverter.TryParseEnum<OrderStatus>(s, out _))
            .WithMessage("Invalid order status value.");

    }
}