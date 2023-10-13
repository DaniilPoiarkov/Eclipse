using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.IdentityUsers;
using Eclipse.Infrastructure.Cache;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Eclipse.Application.Hosted;

internal class EclipseApplicationInizializerHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger _logger;

    public EclipseApplicationInizializerHostedService(IServiceProvider serviceProvider, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Inizializing {module} module", nameof(EclipseApplicationModule));

        using var scope = _serviceProvider.CreateScope();

        _logger.Information("\tRetrieving services");

        var userService = scope.ServiceProvider.GetRequiredService<IIdentityUserService>();
        var userCache = scope.ServiceProvider.GetRequiredService<IIdentityUserCache>();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        _logger.Information("\t\tRetrieving data");

        var users = await userService.GetAllAsync(cancellationToken);

        _logger.Information("\tCaching data");

        foreach (var user in users)
        {
            userCache.AddOrUpdate(user);
            cacheService.Set(new CacheKey($"lang-{user.ChatId}"), string.IsNullOrEmpty(user.Culture) ? "uk" : user.Culture);
        }

        _logger.Information("{module} initialized successfully", nameof(EclipseApplicationModule));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
