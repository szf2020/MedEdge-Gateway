using MedEdge.AiEngine.Services;
using Microsoft.Extensions.Logging;

namespace MedEdge.AiEngine;

public class AiEngineBackgroundService : BackgroundService
{
    private readonly ILogger<AiEngineBackgroundService> _logger;
    private readonly IAnomalyDetector _detector;

    public AiEngineBackgroundService(
        ILogger<AiEngineBackgroundService> logger,
        IAnomalyDetector detector)
    {
        _logger = logger;
        _detector = detector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting AI Clinical Engine");

        // For now, the AI engine monitors telemetry and logs anomalies
        // In Phase 3+, this will be integrated with the full pipeline
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("AI Clinical Engine stopped");
                break;
            }
        }
    }
}
