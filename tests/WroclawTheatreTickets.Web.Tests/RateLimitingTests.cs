namespace WroclawTheatreTickets.Web.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

public class RateLimitingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RateLimitingTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PublicEndpoint_WithinLimit_AllowsRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make 10 requests (well within the 200 per minute limit)
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 10; i++)
        {
            var response = await client.GetAsync("/api/shows/upcoming");
            responses.Add(response);
        }

        // Assert - All should succeed
        Assert.All(responses, response => 
        {
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected OK or BadRequest, got {response.StatusCode}");
        });
    }

    [Fact]
    public async Task RateLimitingConfiguration_PublicPolicyExists()
    {
        // Arrange & Act
        var policyName = RateLimitingConfiguration.PublicEndpointsPolicy;

        // Assert
        Assert.Equal("PublicEndpoints", policyName);
    }

    [Fact]
    public async Task RateLimitingConfiguration_AuthenticatedPolicyExists()
    {
        // Arrange & Act
        var policyName = RateLimitingConfiguration.AuthenticatedEndpointsPolicy;

        // Assert
        Assert.Equal("AuthenticatedEndpoints", policyName);
    }

    [Fact]
    public async Task RateLimitingConfiguration_AdminPolicyExists()
    {
        // Arrange & Act
        var policyName = RateLimitingConfiguration.AdminEndpointsPolicy;

        // Assert
        Assert.Equal("AdminEndpoints", policyName);
    }

    [Fact]
    public async Task PublicEndpoint_SearchEndpoint_WorksWithRateLimiting()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make a few search requests
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 5; i++)
        {
            var response = await client.GetAsync("/api/shows/search?keyword=test");
            responses.Add(response);
        }

        // Assert - Should work within limits
        Assert.All(responses, response => 
        {
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Expected OK or BadRequest, got {response.StatusCode}");
        });
    }

    [Fact]
    public async Task AuthEndpoint_WorksWithRateLimiting()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginRequest = new
        {
            Email = "test@example.com",
            Password = "testpassword"
        };

        // Act - Make a few auth requests
        var responses = new List<HttpResponseMessage>();
        for (int i = 0; i < 5; i++)
        {
            var response = await client.PostAsync("/api/auth/login", 
                new StringContent(
                    JsonSerializer.Serialize(loginRequest),
                    Encoding.UTF8,
                    "application/json"));
            responses.Add(response);
        }

        // Assert - All should be processed (not rate limited at this low volume)
        Assert.All(responses, response => 
        {
            Assert.True(
                response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.Unauthorized,
                $"Expected OK, BadRequest, or Unauthorized, got {response.StatusCode}");
        });
    }
}

