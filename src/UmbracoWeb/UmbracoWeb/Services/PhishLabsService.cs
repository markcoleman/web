using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using UmbracoWeb.Models;

namespace UmbracoWeb.Services;

/// <summary>
/// Service for integrating with PhishLabs incident reporting API
/// </summary>
public class PhishLabsService : IPhishLabsService
{
    private readonly HttpClient _httpClient;
    private readonly PhishLabsSettings _settings;
    private readonly ILogger<PhishLabsService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PhishLabsService(
        HttpClient httpClient, 
        IOptions<PhishLabsSettings> settings, 
        ILogger<PhishLabsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        ConfigureHttpClient();
    }

    /// <summary>
    /// Submit a phishing incident to PhishLabs
    /// </summary>
    public async Task<PhishLabsIncidentResponse> SubmitIncidentAsync(
        PhishLabsIncidentRequest request, 
        string correlationId, 
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(correlationId))
            throw new ArgumentException("Correlation ID is required", nameof(correlationId));

        _logger.LogInformation("Submitting PhishLabs incident. CorrelationId: {CorrelationId}, URL: {UrlHash}", 
            correlationId, ComputeUrlHash(request.Url));

        try
        {
            var apiRequest = new PhishLabsApiRequest
            {
                Url = SanitizeUrl(request.Url),
                Description = SanitizeDescription(request.Details),
                Source = "umbraco-web",
                Timestamp = DateTime.UtcNow
            };

            var response = await SubmitToPhishLabsAsync(apiRequest, correlationId, cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("PhishLabs incident submitted successfully. CorrelationId: {CorrelationId}, IncidentId: {IncidentId}", 
                    correlationId, response.IncidentId);

                return new PhishLabsIncidentResponse
                {
                    Success = true,
                    CorrelationId = correlationId,
                    Message = "Thanks — we received your report and are investigating. If this affects your account, we'll contact you."
                };
            }
            else
            {
                _logger.LogWarning("PhishLabs incident submission failed. CorrelationId: {CorrelationId}, Error: {Error}", 
                    correlationId, response.Error);

                return new PhishLabsIncidentResponse
                {
                    Success = false,
                    CorrelationId = correlationId,
                    Message = "Something went wrong — please try again.",
                    ErrorDetails = response.Error
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting PhishLabs incident. CorrelationId: {CorrelationId}", correlationId);

            return new PhishLabsIncidentResponse
            {
                Success = false,
                CorrelationId = correlationId,
                Message = "Something went wrong — please try again. If it keeps failing, contact support.",
                ErrorDetails = ex.Message
            };
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_settings.ApiBaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Umbraco-Web-PhishLabs-Integration/1.0");
    }

    private async Task<PhishLabsApiResponse> SubmitToPhishLabsAsync(
        PhishLabsApiRequest request, 
        string correlationId, 
        CancellationToken cancellationToken)
    {
        var endpoint = _settings.ServicePath.TrimStart('/');
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        content.Headers.Add("X-Correlation-ID", correlationId);

        var httpResponse = await _httpClient.PostAsync(endpoint, content, cancellationToken);

        if (httpResponse.IsSuccessStatusCode)
        {
            var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<PhishLabsApiResponse>(responseContent, _jsonOptions);
            
            return apiResponse ?? new PhishLabsApiResponse 
            { 
                Success = false, 
                Error = "Invalid response format" 
            };
        }
        else
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            return new PhishLabsApiResponse
            {
                Success = false,
                Error = $"HTTP {(int)httpResponse.StatusCode}: {errorContent}"
            };
        }
    }

    private static string SanitizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        // Basic URL sanitization - remove any dangerous characters
        return url.Trim().Replace("\r", "").Replace("\n", "");
    }

    private static string? SanitizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        // Basic description sanitization
        return description.Trim().Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private static string ComputeUrlHash(string url)
    {
        // Create a simple hash for logging (no PII)
        return url.GetHashCode().ToString("X8");
    }
}