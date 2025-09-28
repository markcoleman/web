using UmbracoWeb.Models;

namespace UmbracoWeb.Services;

/// <summary>
/// Service interface for PhishLabs incident reporting
/// </summary>
public interface IPhishLabsService
{
    /// <summary>
    /// Submit a phishing incident to PhishLabs
    /// </summary>
    /// <param name="request">The incident request</param>
    /// <param name="correlationId">Correlation ID for tracking</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response from PhishLabs</returns>
    Task<PhishLabsIncidentResponse> SubmitIncidentAsync(
        PhishLabsIncidentRequest request, 
        string correlationId, 
        CancellationToken cancellationToken = default);
}