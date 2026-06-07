using Inventory.Application.Common.Exceptions;
using Inventory.Application.Customers.Dtos;
using Inventory.Application.Orders.Commands;
using Inventory.Application.Orders.Queries;
using Inventory.Domain.Enums;
using Inventory.Domain.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders= await _mediator.Send(new GetOrdersQuery());
            return Ok(orders);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));

            return order is null 
                ? throw new NotFoundException($"Order {id} does not exist") 
                : Ok(order);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusRequest request)
        {
            var status = EnumStringConverter.ParseEnumOrThrow<OrderStatus>(request.Status);

            await _mediator.Send(new UpdateOrderStatusCommand(id, status));

            return NoContent();
        }
    }
}
