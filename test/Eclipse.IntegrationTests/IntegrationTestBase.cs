using Bogus;

using Microsoft.Extensions.DependencyInjection;

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

    /// <inheritdoc/>
    public void Dispose()
    {
        Scope.Dispose();
        Client.Dispose();

        GC.SuppressFinalize(this);
    }
}
