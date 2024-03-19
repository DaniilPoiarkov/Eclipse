using FluentAssertions;

namespace Eclipse.IntegrationTests.Ping;

public sealed class PingIntegrationTests : IntegrationTestBase
{
    public PingIntegrationTests(TestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Get_WhenCalled_ThenTextReturned()
    {
        var response = await Client.GetAsync("api/ping");

        var body = await response.Content.ReadAsStringAsync();

        body.Should().Be("Ping");
    }
}
