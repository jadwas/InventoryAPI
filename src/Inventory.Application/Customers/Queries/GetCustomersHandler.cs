using Inventory.Application.Common.Interfaces;
using Inventory.Application.Customers.Dtos;
using Inventory.Domain.Enums;
using MediatR;

namespace Inventory.Application.Customers.Queries;

public class GetCustomersHandler : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly ICustomerRepository _repo;


    public GetCustomersHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repo.GetAllAsync(cancellationToken);

        return customers.Select(customer =>
            new CustomerDto(customer.Id, customer.Name, Region.Europe)
        );
    }
}