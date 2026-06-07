using Inventory.Application.Common.Dtos;
using Inventory.Application.Customers.Dtos;
using Inventory.Domain.Enums;
using System.Net;
using System.Net.Http.Json;
using Inventory.Application.Customers.Commands;

namespace Inventory.Tests.Integration.Api
{
    public class CustomersControllerTests : IClassFixture<TestDatabaseFactory>
    {
        private readonly HttpClient _client;

        public CustomersControllerTests(TestDatabaseFactory factory)
        {
            _client = factory.CreateClient();
        }

        // -----------------------------
        // 1. Create customer
        // -----------------------------
        [Fact]
        public async Task CreateCustomer_ShouldReturnCreated()
        {
            var request = new CreateCustomerCommand(
                Name: "John Doe",
                Region: "Europe"
            );

            var response = await _client.PostAsJsonAsync("/customers", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<IdResponse>();
            Assert.NotEqual(Guid.Empty, body.Id);
        }

        // -----------------------------
        // 2. GetAll customers
        // -----------------------------
        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/customers");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // -----------------------------
        // 3. GetById — existing
        // -----------------------------
        [Fact]
        public async Task GetById_ShouldReturnCustomer_WhenExists()
        {
            // create customer

            var request = new CreateCustomerCommand("Alice", "US");

            var createResponse = await _client.PostAsJsonAsync("/customers", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<IdResponse>();
            // get customer
            var response = await _client.GetAsync($"/customers/{createdCustomer.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();

            Assert.Equal("Alice", customer.Name);
            Assert.Equal(Region.US, customer.Region);
        }

        // -----------------------------
        // 4. GetById — not found
        // -----------------------------
        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNotExists()
        {
            var response = await _client.GetAsync($"/customers/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // -----------------------------
        // 5. Create — invalid region
        // -----------------------------
        [Fact]
        public async Task CreateCustomer_InvalidRegion_ShouldReturnBadRequest()
        {
            var request = new CreateCustomerCommand(
                Name: "Invalid Region User",
                Region: "Atlantis" // ❌ nie istnieje w enum
            );

            var response = await _client.PostAsJsonAsync("/customers", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }


}
