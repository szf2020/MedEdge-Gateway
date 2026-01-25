using MedEdge.Core.DTOs;
using MedEdge.TransformService.Services;
using Microsoft.Extensions.Logging;

namespace MedEdge.TransformService;

public class TransformServiceBackgroundTask : BackgroundService
{
    private readonly ILogger<TransformServiceBackgroundTask> _logger;
    private readonly IServiceProvider _serviceProvider;

    public TransformServiceBackgroundTask(
        ILogger<TransformServiceBackgroundTask> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting transform service background task");

        using var scope = _serviceProvider.CreateScope();
        var mapper = scope.ServiceProvider.GetRequiredService<TelemetryToObservationMapper>();

        try
        {
            await mapper.MapAndPersistAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Transform service stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in transform service");
        }
    }
}
