using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedEdge.Core.DTOs;
using Polly;
using System.Text.Json;
using System.Text;

namespace MedEdge.TransformService.Services;

public class FhirApiClientOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5000";
    public int TimeoutSeconds { get; set; } = 30;
}

public interface IFhirApiClient
{
    Task CreateObservationAsync(CreateObservationRequest request, CancellationToken cancellationToken = default);
}

public class FhirApiClient : IFhirApiClient
{
    private readonly ILogger<FhirApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly FhirApiClientOptions _options;
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    public FhirApiClient(
        ILogger<FhirApiClient> logger,
        HttpClient httpClient,
        IOptions<FhirApiClientOptions> options)
    {
        _logger = logger;
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

        // Setup retry policy
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, duration, attempt, _) =>
                {
                    _logger.LogWarning(
                        "FHIR API call retry attempt {Attempt} after {Duration}",
                        attempt,
                        duration
                    );
                }
            );
    }

    public async Task CreateObservationAsync(
        CreateObservationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.PostAsync("/fhir/Observation", content, cancellationToken);
            });

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug(
                    "Created observation {Code} for patient {PatientId}",
                    request.Code,
                    request.PatientId
                );
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Failed to create observation: {StatusCode} - {Error}",
                    response.StatusCode,
                    error
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating observation");
            throw;
        }
    }
}
