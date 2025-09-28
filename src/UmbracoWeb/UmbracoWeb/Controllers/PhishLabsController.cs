using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UmbracoWeb.Models;
using UmbracoWeb.Services;

namespace UmbracoWeb.Controllers;

/// <summary>
/// API controller for PhishLabs incident reporting
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PhishLabsController : ControllerBase
{
    private readonly IPhishLabsService _phishLabsService;
    private readonly ILogger<PhishLabsController> _logger;

    public PhishLabsController(
        IPhishLabsService phishLabsService, 
        ILogger<PhishLabsController> logger)
    {
        _phishLabsService = phishLabsService ?? throw new ArgumentNullException(nameof(phishLabsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Submit a phishing incident report
    /// </summary>
    /// <param name="request">The incident request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response indicating success or failure</returns>
    [HttpPost("incidents")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<PhishLabsIncidentResponse>> SubmitIncident(
        [FromBody] PhishLabsIncidentRequest request,
        CancellationToken cancellationToken = default)
    {
        // Generate correlation ID for this request
        var correlationId = Guid.NewGuid().ToString("N")[..12];

        // Get client IP for logging (but don't store PII)
        var clientIp = GetClientIpHash();

        _logger.LogInformation("PhishLabs incident submission started. CorrelationId: {CorrelationId}, ClientIP: {ClientIpHash}", 
            correlationId, clientIp);

        // Validate model state
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid PhishLabs incident request. CorrelationId: {CorrelationId}, Errors: {Errors}", 
                correlationId, string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            return BadRequest(new PhishLabsIncidentResponse
            {
                Success = false,
                CorrelationId = correlationId,
                Message = "Please check your input and try again.",
                ErrorDetails = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
            });
        }

        try
        {
            // Submit to PhishLabs
            var response = await _phishLabsService.SubmitIncidentAsync(request, correlationId, cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("PhishLabs incident submitted successfully. CorrelationId: {CorrelationId}", correlationId);
                return Ok(response);
            }
            else
            {
                _logger.LogWarning("PhishLabs incident submission failed. CorrelationId: {CorrelationId}", correlationId);
                return StatusCode(500, response);
            }
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error in PhishLabs incident submission. CorrelationId: {CorrelationId}", correlationId);

            return BadRequest(new PhishLabsIncidentResponse
            {
                Success = false,
                CorrelationId = correlationId,
                Message = "Please check your input and try again.",
                ErrorDetails = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in PhishLabs incident submission. CorrelationId: {CorrelationId}", correlationId);

            return StatusCode(500, new PhishLabsIncidentResponse
            {
                Success = false,
                CorrelationId = correlationId,
                Message = "Something went wrong — please try again. If it keeps failing, contact support.",
                ErrorDetails = "Internal server error"
            });
        }
    }

    /// <summary>
    /// Health check endpoint for the PhishLabs integration
    /// </summary>
    [HttpGet("health")]
    public ActionResult<object> Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    private string GetClientIpHash()
    {
        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // Hash the IP for privacy - don't store actual IPs
        return remoteIp.GetHashCode().ToString("X8");
    }
}