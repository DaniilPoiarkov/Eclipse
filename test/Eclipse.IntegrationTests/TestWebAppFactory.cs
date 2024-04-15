using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NSubstitute;

using Telegram.Bot;

using Testcontainers.CosmosDb;

namespace Eclipse.IntegrationTests;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _dbContainer = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        .WithName($"eclipse_cosmosdb_{Guid.NewGuid()}")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ConfigureCosmosClient(services);
            ConfigureTelegramBot(services);
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }

    #region Services Configuration

    private static void ConfigureTelegramBot(IServiceCollection services)
    {
        services.RemoveAll<ITelegramBotClient>();

        services.AddSingleton(sp => Substitute.For<ITelegramBotClient>());
    }

    private void ConfigureCosmosClient(IServiceCollection services)
    {
        services.RemoveAll<CosmosClient>();

        var clientOptions = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            HttpClientFactory = () => _dbContainer.HttpClient,
        };

        services.AddSingleton(
            new CosmosClient(_dbContainer.GetConnectionString(), clientOptions)
        );
    }

    #endregion
}
