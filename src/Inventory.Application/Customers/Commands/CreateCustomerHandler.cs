using Inventory.Application.Common.Dtos;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using MediatR;

namespace Inventory.Application.Customers.Commands;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IdResponse>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerCommandHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<IdResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Region = Enum.Parse<Region>(request.Region, true)
        };

        await _repo.AddAsync(customer, cancellationToken);

        return new IdResponse(customer.Id);
    }
}