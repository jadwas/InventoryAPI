using Inventory.Application.Common.Exceptions;
using Inventory.Application.Products.Commands;
using Inventory.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));

            return product is null 
                ? throw new NotFoundException($"Product {id} does not exist") 
                : Ok(product);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            var deleted = await _mediator.Send(new DeleteProductByIdCommand(id));

            return !deleted 
                ? throw new NotFoundException($"Product {id} does not exist") 
                : NoContent();
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id,[FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                throw new BadRequestException($"Product ID ({id}) and Product ID from message ({command.Id}) does not match.");

            var updated = await _mediator.Send(command);

            return !updated 
                ? throw new NotFoundException($"Product {id} does not exist") 
                : NoContent();
        }
        
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var updated = await _mediator.Send(new ActivateProductByIdCommand(id));
            return !updated
                ? throw new NotFoundException($"Product {id} does not exist")
                : NoContent();
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var updated = await _mediator.Send(new DeactivateProductByIdCommand(id));
            return !updated
                ? throw new NotFoundException($"Product {id} does not exist")
                : NoContent();

        }

    }
}
