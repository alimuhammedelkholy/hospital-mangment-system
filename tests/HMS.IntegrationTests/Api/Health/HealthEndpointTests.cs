using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
namespace HMS.IntegrationTests;
public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public HealthEndpointTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();
    [Fact] public async Task Health_returns_success() { using var response = await _client.GetAsync("/health"); response.EnsureSuccessStatusCode(); }
}
