using Inventory.Application.Common.Dtos;
using Inventory.Application.Orders.Commands;
using Inventory.Application.Orders.Dtos;
using Inventory.Application.Products.Commands;
using System.Net;
using System.Net.Http.Json;
using Inventory.Application.Customers.Commands;

namespace Inventory.Tests.Integration.Api
{


    public class GenericControllerTests : IClassFixture<TestDatabaseFactory>
    {
        private readonly HttpClient _client;

        public GenericControllerTests(TestDatabaseFactory factory)
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
        // Not found on non-existing endpoint
        // -----------------------------
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            var (customer, product) = await CreateCustomerAndProduct();

            // Create order
            var orderCommand = new CreateOrderCommand(
                CustomerId: customer.Id,
                Items: [new CreateOrderItemDto(product.Id, 2)]
            );


            var createResponse = await _client.PostAsJsonAsync("/orders", orderCommand);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<IdResponse>();

            var response = await _client.GetAsync($"/orderss/{createdOrder.Id}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            response = await _client.GetAsync($"/orders/{createdOrder.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
