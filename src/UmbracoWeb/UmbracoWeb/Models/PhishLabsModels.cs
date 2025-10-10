using System.ComponentModel.DataAnnotations;

namespace UmbracoWeb.Models;

/// <summary>
/// Request model for submitting a phishing incident
/// </summary>
public class PhishLabsIncidentRequest
{
    /// <summary>
    /// The suspicious URL to report
    /// </summary>
    [Required(ErrorMessage = "URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Optional details about the incident (max 1000 characters)
    /// </summary>
    [StringLength(1000, ErrorMessage = "Details cannot exceed 1000 characters")]
    public string? Details { get; set; }
}

/// <summary>
/// Response model for PhishLabs incident submission
/// </summary>
public class PhishLabsIncidentResponse
{
    /// <summary>
    /// Indicates if the submission was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Correlation ID for tracking the request
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Message to display to the user
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error details if submission failed
    /// </summary>
    public string? ErrorDetails { get; set; }
}

/// <summary>
/// Internal model for PhishLabs Case Creation API request
/// </summary>
internal class PhishLabsApiRequest
{
    public string CaseType { get; set; } = "Phishing";
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = "umbraco-web";
}

/// <summary>
/// Internal model for PhishLabs Case Creation API response
/// </summary>
internal class PhishLabsApiResponse
{
    public bool Success { get; set; }
    public string? CaseId { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}