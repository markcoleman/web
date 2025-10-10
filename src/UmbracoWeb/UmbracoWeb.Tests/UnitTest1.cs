using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using UmbracoWeb.Models;
using UmbracoWeb.Services;

namespace UmbracoWeb.Tests;

[TestFixture]
public class PhishLabsServiceTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private Mock<ILogger<PhishLabsService>> _loggerMock;
    private PhishLabsSettings _settings;
    private PhishLabsService _service;

    [SetUp]
    public void SetUp()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<PhishLabsService>>();
        
        _settings = new PhishLabsSettings
        {
            ApiBaseUrl = "https://caseapi.phishlabs.com",
            ApiKey = "test-key",
            ServicePath = "/v1/create",
            TimeoutSeconds = 30,
            MaxRetries = 3,
            RateLimitPerMinute = 10
        };

        var options = Options.Create(_settings);
        _service = new PhishLabsService(_httpClient, options, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task SubmitIncidentAsync_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new PhishLabsIncidentRequest
        {
            Url = "https://suspicious-site.com/phishing",
            Details = "Found this link in a suspicious email"
        };

        var correlationId = "test-correlation-123";

        var apiResponse = new
        {
            success = true,
            caseId = "case-456",
            message = "Case created successfully"
        };

        var responseContent = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.SubmitIncidentAsync(request, correlationId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.True);
        Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        Assert.That(result.Message, Contains.Substring("Thanks — we received your report"));
    }

    [Test]
    public async Task SubmitIncidentAsync_WithHttpError_ReturnsFailureResponse()
    {
        // Arrange
        var request = new PhishLabsIncidentRequest
        {
            Url = "https://suspicious-site.com/phishing"
        };

        var correlationId = "test-correlation-456";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
            });

        // Act
        var result = await _service.SubmitIncidentAsync(request, correlationId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        Assert.That(result.Message, Contains.Substring("Something went wrong"));
    }

    [Test]
    public void SubmitIncidentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var correlationId = "test-correlation-789";

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _service.SubmitIncidentAsync(null!, correlationId));
    }

    [Test]
    public void SubmitIncidentAsync_WithEmptyCorrelationId_ThrowsArgumentException()
    {
        // Arrange
        var request = new PhishLabsIncidentRequest
        {
            Url = "https://suspicious-site.com/phishing"
        };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.SubmitIncidentAsync(request, string.Empty));
    }

    [Test]
    public async Task SubmitIncidentAsync_WithNetworkException_ReturnsFailureResponse()
    {
        // Arrange
        var request = new PhishLabsIncidentRequest
        {
            Url = "https://suspicious-site.com/phishing"
        };

        var correlationId = "test-correlation-network";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _service.SubmitIncidentAsync(request, correlationId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
        Assert.That(result.Message, Contains.Substring("Something went wrong"));
    }
}
