using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eclipse.DataAccess.Migrations;

internal sealed class MigrationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            await migrator.Migrate(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new MigrationException("Failed to run migrations.", ex);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
