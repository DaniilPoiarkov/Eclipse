using Bogus;

using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.IntegrationTests;

public abstract class IntegrationTestBase : IClassFixture<TestWebAppFactory>, IDisposable
{
    protected readonly IServiceScope Scope;

    protected readonly HttpClient Client;

    protected readonly Faker Faker;

    protected IntegrationTestBase(TestWebAppFactory factory)
    {
        Scope = factory.Services.CreateScope();
        Client = factory.CreateClient();

        var apiKeyOptions = Scope.ServiceProvider
            .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        Client.DefaultRequestHeaders.Add("X-API-KEY", apiKeyOptions.Value.EclipseApiKey);

        Faker = new Faker();
    }

    public void Dispose()
    {
        Scope.Dispose();
        Client.Dispose();

        GC.SuppressFinalize(this);
    }
}
