using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Tests.Integration.Api;

public class CorrelationIdIntegrationTests : IClassFixture<TestDatabaseFactory>
{
    private readonly HttpClient _client;

    public CorrelationIdIntegrationTests(TestDatabaseFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CorrelationId_ShouldBeReturned_AndRemainConsistentWithinRequest()
    {
        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));

        var correlationId = values.First();
        Assert.False(string.IsNullOrWhiteSpace(correlationId));

        var response2 = await _client.GetAsync("/products");
        Assert.True(response2.Headers.TryGetValues("X-Correlation-ID", out var values2));

        var correlationId2 = values2.First();
        Assert.DoesNotMatch(correlationId,correlationId2);
    }

    [Fact]
    public async Task CorrelationId_ShouldPropagateIntoValidationProblemDetails()
    {
        // Arrange 
        var invalidRequest = new
        {
            Name = "",          // invalid
            Description = "",   // invalid
            Price = -1,         // invalid
            Stock = -5          // invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));

        var correlationId = values.First();

        
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.False(string.IsNullOrWhiteSpace(correlationId));
        Assert.True(problem!.Extensions.ContainsKey("CID"));

        Assert.Equal(correlationId,problem.Extensions["CID"]!.ToString());
        Assert.True(problem.Errors.ContainsKey("Name"));
        Assert.True(problem.Errors.ContainsKey("Description"));
        Assert.True(problem.Errors.ContainsKey("Price"));
        Assert.True(problem.Errors.ContainsKey("Stock"));
    }
}
