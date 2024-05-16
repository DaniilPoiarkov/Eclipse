using FluentAssertions;

namespace Eclipse.IntegrationTests.Ping;

public sealed class PingIntegrationTests : IntegrationTestBase
{
    public PingIntegrationTests(WebAppFactoryWithTestcontainers factory)
        : base(factory) { }

    //[Theory]
    //[InlineData("1", "Ping")]
    //[InlineData("2", "Pong")]
    public async Task Get_WhenCalledWithVersion_ThenProperResponseReturned(string version, string expectedResult)
    {
        Client.DefaultRequestHeaders.Add("X-Api-Version", version);

        using var response = await Client.GetAsync($"api/ping");

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Be(expectedResult);
    }
}
