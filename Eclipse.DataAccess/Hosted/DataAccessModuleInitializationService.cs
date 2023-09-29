using Eclipse.DataAccess.EclipseCosmosDb;

using Microsoft.Extensions.Hosting;

using Serilog;

namespace Eclipse.DataAccess.Hosted;

internal class DataAccessModuleInitializationService : IHostedService
{
    private readonly ILogger _logger;

    private readonly EclipseCosmosDbContext _context;
    public DataAccessModuleInitializationService(ILogger logger, EclipseCosmosDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Initializing {module} module", nameof(EclipseDataAccessModule));

        _logger.Information("\tInitializing database");
        await _context.InitializeAsync(cancellationToken);

        _logger.Information("{module} module initialized successfully", nameof(EclipseDataAccessModule));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
