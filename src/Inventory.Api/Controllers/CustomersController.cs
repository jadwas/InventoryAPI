using Inventory.Application.Common.Exceptions;
using Inventory.Application.Customers.Commands;
using Inventory.Application.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers= await _mediator.Send(new GetCustomersQuery());
            return Ok(customers);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var customer = await _mediator.Send(new GetCustomerByIdQuery(id));

            return customer is null 
                ? throw new NotFoundException($"Customer {id} does not exist") 
                : Ok(customer);
        }
    }
}
