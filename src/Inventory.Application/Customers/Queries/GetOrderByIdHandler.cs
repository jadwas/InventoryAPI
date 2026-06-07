using Inventory.Application.Common.Interfaces;
using Inventory.Application.Customers.Dtos;
using MediatR;

namespace Inventory.Application.Customers.Queries;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repo.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
            return null;

        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Region);
    }
}