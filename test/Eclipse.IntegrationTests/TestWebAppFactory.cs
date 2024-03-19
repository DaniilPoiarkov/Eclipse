using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Testcontainers.CosmosDb;

namespace Eclipse.IntegrationTests;

public sealed class TestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _dbContainer = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureCosmosClient);
    }

    private void ConfigureCosmosClient(IServiceCollection services)
    {
        services.RemoveAll<CosmosClient>();

        var clientOptions = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            HttpClientFactory = () => _dbContainer.HttpClient,
        };

        services.AddSingleton(new CosmosClient(_dbContainer.GetConnectionString(), clientOptions));
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
