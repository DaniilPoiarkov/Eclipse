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
public abstract class IntegrationTestBase : IClassFixture<WebAppFactoryWithTestcontainers>, IDisposable
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

    protected IntegrationTestBase(WebAppFactoryWithTestcontainers factory)
    {
        Scope = factory.Services.CreateScope();
        Client = factory.CreateClient();
        Faker = new Faker();
    }

    /// <summary>
    /// Adds defined by application <b>"X-API-KEY"</b> header to access api authorized via api-key.
    /// <para>Present by default.</para>
    /// </summary>
    protected void AddAppAuthorizationHeader()
    {
        var apiKeyOptions = Scope.ServiceProvider
            .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>();

        Client.DefaultRequestHeaders.Add("X-Api-Key", apiKeyOptions.Value.EclipseApiKey);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Scope.Dispose();
        Client.Dispose();

        GC.SuppressFinalize(this);
    }
}
