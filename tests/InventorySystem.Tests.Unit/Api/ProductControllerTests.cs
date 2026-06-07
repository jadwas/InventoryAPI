using Inventory.Application.Common.Dtos;
using Inventory.Application.Customers.Commands;
using Inventory.Application.Products.Commands;
using Inventory.Application.Products.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace Inventory.Tests.Integration.Api;

public class ProductControllerTests : IClassFixture<TestDatabaseFactory>
{
    private readonly HttpClient _client;

    public ProductControllerTests(TestDatabaseFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ------------------------------------------------------------
    // CREATE
    // ------------------------------------------------------------

    [Fact]
    public async Task CreateProduct_ShouldReturnCreated()
    {
        var request = new CreateProductCommand(
            Name: "Laptop",
            Description: "Test product",
            Price: 1000m,
            Stock: 10
        );

        var response = await _client.PostAsJsonAsync("/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotEqual(Guid.Empty, product.Id);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnValidationError()
    {
        var request = new CreateProductCommand(
            Name: "Laptop",
            Description: "",
            Price: 1000m,
            Stock: -10
        );

        var response = await _client.PostAsJsonAsync("/products", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.True(problem.Errors.Keys.Any(a=> a.Equals("stock", StringComparison.InvariantCultureIgnoreCase)));
        Assert.True(problem.Errors.Keys.Any(a => a.Equals("description", StringComparison.InvariantCultureIgnoreCase)));
    }

    // ------------------------------------------------------------
    // GET SINGLE
    // ------------------------------------------------------------

    [Fact]
    public async Task GetProduct_ShouldReturnProduct()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        
        var response = await _client.GetAsync($"/products/{created!.Id}");

        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal("A", product!.Name);
    }

    [Fact]
    public async Task GetProduct_ShouldReturn404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(404, problem!.Status);
        Assert.True(problem.Detail.Contains("does not exist"));
    }

    // ------------------------------------------------------------
    // GET LIST
    // ------------------------------------------------------------

    [Fact]
    public async Task GetProducts_ShouldReturnList()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        await _client.PostAsJsonAsync("/products", request);
        request = new CreateProductCommand(
            Name: "B",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        await _client.PostAsJsonAsync("/products", request);

        var response = await _client.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var list = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.True(2<list.Count);
        
    }

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------

    [Fact]
    public async Task UpdateProduct_ShouldReturnNoContent()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        var update = new UpdateProductCommand(created.Id, "Updated", "Description", 10, 10);

        var response = await _client.PutAsJsonAsync($"/products/{created.Id}", update);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateStatus_Activate_ShouldReturnNoContent()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.PatchAsync($"/products/{created.Id}/activate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_Deactivate_ShouldReturnNoContent()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        var update = new UpdateProductCommand(created.Id, "Updated", "Description", 10, 10);

        var response = await _client.PatchAsync($"/products/{created.Id}/deactivate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturn400_WhenIdMismatch()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        var update = new UpdateProductCommand(Guid.NewGuid(), "Updated", "", 10, 10);

        var response = await _client.PutAsJsonAsync($"/products/{created!.Id}", update);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturn404_WhenNotFound()
    {
        var productId = Guid.NewGuid();
        var update = new UpdateProductCommand(productId, "X", "D", 10, 10);

        var response = await _client.PutAsJsonAsync($"/products/{productId}", update);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task UpdateProductStatus_Activate_ShouldReturn404_WhenNotFound()
    {
        var productId = Guid.NewGuid();

        var response = await _client.PatchAsync($"/products/{productId}/activate", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateProductStatus_Deactivate_ShouldReturn404_WhenNotFound()
    {
        var productId = Guid.NewGuid();

        var response = await _client.PatchAsync($"/products/{productId}/deactivate", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ------------------------------------------------------------
    // DELETE
    // ------------------------------------------------------------

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent()
    {
        var request = new CreateProductCommand(
            Name: "A",
            Description: "Test product",
            Price: 1000m,
            Stock: 100
        );
        var create = await _client.PostAsJsonAsync("/products", request);
        var created = await create.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.DeleteAsync($"/products/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturn404_WhenNotFound()
    {
        var response = await _client.DeleteAsync($"/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal((int)HttpStatusCode.NotFound, problem!.Status);
        Assert.True(problem.Detail.Contains("does not exist"));
    }

    [Fact]
    public async Task DeleteProduct_WhenUsedInOrder_ReturnsBadRequest()
    {
        var createProduct = new
        {
            Name = "Test Product",
            Description = "Test Desc",
            Price = 10m,
            Stock = 100
        };

        var productResponse = await _client.PostAsJsonAsync("/products", createProduct);
        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);

        var product = await productResponse.Content.ReadFromJsonAsync<IdResponse>();
        Assert.NotEqual(Guid.Empty, product?.Id);
        
        
        var customerRequest = new CreateCustomerCommand(
            Name: "John Doe",
            Region: "Europe"
        );

        var customerResponse = await _client.PostAsJsonAsync("/customers", customerRequest);
        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);
        
        var customer = await customerResponse.Content.ReadFromJsonAsync<IdResponse>();
        
        Assert.NotEqual(Guid.Empty, customer?.Id);
        var createOrder = new
        {
            CustomerId = customer.Id, 
            Items = new[]
            {
                new { ProductId = product?.Id, Quantity = 2 }
            }
        };

        var orderResponse = await _client.PostAsJsonAsync("/orders", createOrder);
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        var deleteResponse = await _client.DeleteAsync($"/products/{product.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);

        var problem = await deleteResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal((int)HttpStatusCode.BadRequest, problem!.Status);
        Assert.True(problem.Errors.Values.SelectMany(v => v).Any(s=>s.Contains("Cannot delete product")));
    }
}
