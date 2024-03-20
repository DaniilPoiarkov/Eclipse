using Bogus;

using Eclipse.WebAPI.Filters.Authorization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Eclipse.IntegrationTests;

/// <summary>
/// Base class for each integration test
/// </summary>
/// <seealso cref="IClassFixture&lt;TestWebAppFactory&gt;" />
/// <seealso cref="IDisposable" />
public abstract class IntegrationTestBase : IClassFixture<TestWebAppFactory>, IDisposable
{
    /// <summary>
    /// The scope of current test execution.
    /// </summary>
    protected readonly IServiceScope Scope;

    /// <summary>
    /// The client which calls in memory instance. No need to dispose it manually.
    /// </summary>
    protected readonly HttpClient Client;

    /// <summary>
    /// The faker to simplify dummy data creation.
    /// </summary>
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
