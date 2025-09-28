using System.ComponentModel.DataAnnotations;

namespace UmbracoWeb.Models;

/// <summary>
/// Configuration settings for PhishLabs API integration
/// </summary>
public class PhishLabsSettings
{
    public const string SectionName = "PhishLabs";

    /// <summary>
    /// Base URL for the PhishLabs API
    /// </summary>
    [Required]
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// API key for authenticating with PhishLabs
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Service path for incidents endpoint
    /// </summary>
    [Required]
    public string ServicePath { get; set; } = string.Empty;

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    [Range(5, 300)]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of retry attempts
    /// </summary>
    [Range(0, 10)]
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Rate limit per minute per user
    /// </summary>
    [Range(1, 100)]
    public int RateLimitPerMinute { get; set; } = 10;
}