using Eclipse.Domain.Shared.Repositories;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NSubstitute;

using Telegram.Bot;

using Testcontainers.CosmosDb;
using Testcontainers.Redis;

namespace Eclipse.IntegrationTests;

public sealed class WebAppFactoryWithTestcontainers : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly CosmosDbContainer _dbContainer = new CosmosDbBuilder()
        .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
        .WithName($"eclipse_cosmosdb_{Guid.NewGuid()}")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis/redis-stack-server:latest")
        .WithName("eclipse_redis")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ConfigureCosmosClient(services);
            ConfigureTelegramBot(services);
            ConfigureRedis(services);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }

    #region Services Configuration

    private void ConfigureRedis(IServiceCollection services)
    {
        services.RemoveAll<IDistributedCache>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = _redisContainer.GetConnectionString();
        });
    }

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
