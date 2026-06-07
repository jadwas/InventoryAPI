using Inventory.Application.Common.Dtos;
using Inventory.Application.Customers.Commands;
using Inventory.Application.Customers.Dtos;
using Inventory.Application.Orders.Commands;
using Inventory.Application.Orders.Dtos;
using Inventory.Application.Products.Commands;
using Inventory.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace Inventory.Tests.Integration.Api
{
    public class OrdersControllerTests : IClassFixture<TestDatabaseFactory>
    {
        private readonly HttpClient _client;

        public OrdersControllerTests(TestDatabaseFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<(IdResponse? customer, IdResponse? product)> CreateCustomerAndProduct()
        {
            // 1. Create customer
            var customerRequest = new CreateCustomerCommand(
                Name: "John Doe",
                Region: "Europe"
            );

            var customerResponse = await _client.PostAsJsonAsync("/customers", customerRequest);
            var customer = await customerResponse.Content.ReadFromJsonAsync<IdResponse>();

            // 2. Create product
            var productRequest = new CreateProductCommand(
                Name: "Laptop",
                Description: "Test product",
                Price: 1000m,
                Stock: 10
            );

            var productResponse = await _client.PostAsJsonAsync("/products", productRequest);
            var product = await productResponse.Content.ReadFromJsonAsync<IdResponse>();
            return (customer, product);
        }

        // -----------------------------
        // 1. Create Order
        // -----------------------------
        [Fact]
        public async Task CreateOrder_ShouldReturnCreated()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            
            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2)]
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

            var body = await orderResponse.Content.ReadFromJsonAsync<OrderDetailsDto>();
            Assert.NotNull(body.Id);
        }
        
        [Fact]
        public async Task CreateOrder_InactiveProduct_ReturnsUnprocessableEntity()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            var response = await _client.PatchAsync($"/products/{product.Id}/deactivate", null);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2)]
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, orderResponse.StatusCode);

           
        }
        
        [Fact]
        public async Task CreateOrder_NotEnoughStock_ReturnsUnprocessableEntity()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            
            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 20)]
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, orderResponse.StatusCode);

           
        }
        
        [Fact]
        public async Task CreateOrder_NonGroupedProducts__ReturnsBadRequest()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            
            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2), new CreateOrderItemDto(product.Id, 5)]
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, orderResponse.StatusCode);


            var problem = await orderResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problem);

            Assert.Equal((int)HttpStatusCode.BadRequest, problem!.Status);
            Assert.True(problem.Errors.Values.SelectMany(v => v).Any(s => s.Contains("Products has to be grouped")));
        }
   
        [Fact]
        public async Task CreateOrder_NonExistingCustomer__ReturnsBadRequest()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            
            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: Guid.NewGuid(),
                Items: [new CreateOrderItemDto(product.Id, 2), new CreateOrderItemDto(product.Id, 5)]
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, orderResponse.StatusCode);


            var problem = await orderResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problem);

            Assert.Equal((int)HttpStatusCode.BadRequest, problem!.Status);
            Assert.True(problem.Errors.Values.SelectMany(v => v).Any(s => s.Contains("Products has to be grouped")));
        }

        [Fact]
        public async Task CreateOrder_NonProducts__ReturnsBadRequest()
        {
            var (customer, product) = await CreateCustomerAndProduct();
            
            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: []
            );

            var orderResponse = await _client.PostAsJsonAsync("/orders", orderCommand);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, orderResponse.StatusCode);


            var problem = await orderResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problem);

            Assert.Equal((int)HttpStatusCode.BadRequest, problem!.Status);
            Assert.True(problem.Errors.Values.SelectMany(v => v).Any(s => s.Contains("Order must contain at least one item.")));
        }


        // -----------------------------
        // 2. GetAll Orders
        // -----------------------------
        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/orders");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // -----------------------------
        // 3. GetById
        // -----------------------------
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // -----------------------------
        // 4. UpdateStatus
        // -----------------------------
        [Fact]
        public async Task UpdateStatus_ShouldUpdateOrderStatus()
        {
            var (customer, product) = await CreateCustomerAndProduct();

            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2)]
            );
           

            var createResponse = await _client.PostAsJsonAsync("/orders", orderCommand);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
          

            // Attempt to update status
            var update = new UpdateOrderStatusRequest(createdOrder.Id, "processing");

            var patchResponse = await _client.PatchAsJsonAsync($"/orders/{createdOrder.Id}/status", update);

            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            //Verify
            var getResponse = await _client.GetAsync($"/orders/{createdOrder.Id}");
            var order = await getResponse.Content.ReadFromJsonAsync<OrderDetailsDto>();

            Assert.Equal(OrderStatus.Processing, order.Status);
        }
        // -----------------------------
        // 5. UpdateStatus - not allowed transition
        // -----------------------------
        [Fact]
        public async Task UpdateStatus_NotAllowedTransition()
        {
            var (customer, product) = await CreateCustomerAndProduct();

            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2)]
            );

            var createResponse = await _client.PostAsJsonAsync("/orders", orderCommand);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
          
            // Attempt to update status
            var update = new UpdateOrderStatusRequest(createdOrder.Id, "completed");
            var patchResponse = await _client.PatchAsJsonAsync($"/orders/{createdOrder.Id}/status", update);

            Assert.Equal(HttpStatusCode.UnprocessableContent, patchResponse.StatusCode);

            // Verify
            var getResponse = await _client.GetAsync($"/orders/{createdOrder.Id}");
            var order = await getResponse.Content.ReadFromJsonAsync<OrderDetailsDto>();

            Assert.Equal(OrderStatus.New, order.Status);
        }
    }
}
