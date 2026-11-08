using System.Net;
using FluentAssertions;

namespace BullMarket.Api.Tests;

public class HealthAndDocsEndpointsTests : IClassFixture<BullMarketApiFactory>
{
    private readonly HttpClient _client;

    public HealthAndDocsEndpointsTests(BullMarketApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LiveHealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/health/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task OpenApiJson_ReturnsOkAndValidTitle()
    {
        var response = await _client.GetAsync("/openapi/v1.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("BullMarket AI Store API");
    }

    [Fact]
    public async Task ScalarApiReference_ReturnsSuccessOrRedirect()
    {
        var response = await _client.GetAsync("/scalar/v1");

        ((int)response.StatusCode).Should().BeInRange(200, 399);
    }
}
