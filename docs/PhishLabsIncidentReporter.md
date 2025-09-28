# PhishLabs Incident Reporter Component

The PhishLabs Incident Reporter is an Umbraco component that allows members to report suspicious URLs to PhishLabs for security analysis.

## Overview

This component provides:
- A member-friendly UI for reporting suspicious URLs
- Server-side proxy API that securely forwards reports to PhishLabs
- Real-time validation and user feedback
- Accessibility features and responsive design
- Comprehensive logging and error handling

## Architecture

```
Browser → Umbraco Partial View → API Controller → PhishLabs Service → PhishLabs API
                    ↓                    ↓                ↓
              Client-side JS      CSRF Protection   Logging & Telemetry
```

## Installation

### 1. Configuration

Add the PhishLabs settings to your `appsettings.json`:

```json
{
  "PhishLabs": {
    "ApiBaseUrl": "https://api.phishlabs.com",
    "ApiKey": "your-api-key-here",
    "ServicePath": "/incidents/your-service",
    "TimeoutSeconds": 30,
    "MaxRetries": 3,
    "RateLimitPerMinute": 10
  }
}
```

### 2. Environment Variables

For production, set these environment variables:

- `PHISHLABS_API_BASE_URL` - PhishLabs API base URL
- `PHISHLABS_API_KEY` - Your PhishLabs API key
- `PHISHLABS_SERVICE_PATH` - Service endpoint path

### 3. Include in Templates

Add the partial view to any Umbraco template:

```html
@Html.Partial("PhishLabsIncidentReporter")
```

Or in newer Umbraco versions:

```html
@await Html.PartialAsync("PhishLabsIncidentReporter")
```

## API Endpoints

### POST /api/phishlabs/incidents

Submit a phishing incident report.

**Request Body:**
```json
{
  "url": "https://suspicious-site.com/phishing",
  "details": "Optional description of the incident"
}
```

**Response (Success):**
```json
{
  "success": true,
  "correlationId": "abc123def456",
  "message": "Thanks — we received your report and are investigating. If this affects your account, we'll contact you."
}
```

**Response (Error):**
```json
{
  "success": false,
  "correlationId": "abc123def456",
  "message": "Something went wrong — please try again.",
  "errorDetails": "Detailed error information"
}
```

### GET /api/phishlabs/health

Health check endpoint for monitoring.

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Security Features

### Input Validation
- URL format validation
- Maximum length limits (URL: 2048 chars, Details: 1000 chars)
- Server-side sanitization of all inputs

### Authentication & Authorization
- CSRF protection using anti-forgery tokens
- API key stored securely in configuration
- No client-side exposure of sensitive data

### Rate Limiting
- Configurable per-minute limits
- IP-based throttling
- Graceful degradation with user feedback

### Privacy
- URL hashing in logs (no PII stored)
- IP address hashing for logging
- Correlation IDs for request tracing

## Accessibility Features

### ARIA Support
- `role="alert"` for error messages
- `aria-live="polite"` for status updates
- `aria-describedby` for form field descriptions
- Proper labeling and required field indicators

### Keyboard Navigation
- Full keyboard accessibility
- Focus management during form submission
- Logical tab order

### Screen Reader Support
- Descriptive labels and help text
- Status announcements for form submission
- Error message associations

## Customization

### Styling
The component includes comprehensive CSS that can be customized:
- Responsive design (mobile-first)
- Customizable color scheme
- Consistent with Bootstrap patterns

### JavaScript
Client-side functionality can be extended:
- Real-time validation
- Character counting
- Async form submission
- User feedback

### Server-side
The service layer can be extended for:
- Custom logging requirements
- Additional validation rules
- Integration with other security tools

## Monitoring & Logging

### Structured Logging
All operations generate structured log events with:
- Correlation IDs for request tracing
- Sanitized URL hashes (no PII)
- Client IP hashes for security analysis
- Success/failure metrics

### Log Levels
- **Information**: Successful submissions
- **Warning**: Validation failures, API errors
- **Error**: Unexpected exceptions, network failures

### Metrics
Monitor these key metrics:
- Submission success rate
- Average response time
- Error categories and frequencies
- Rate limiting events

## Testing

### Unit Tests
Run the test suite:
```bash
cd src/UmbracoWeb/UmbracoWeb.Tests
dotnet test
```

Tests cover:
- Service layer functionality
- Model validation
- Error handling scenarios
- Configuration validation

### Integration Testing
For end-to-end testing:
1. Configure test PhishLabs API credentials
2. Test form submission flow
3. Verify logging and telemetry
4. Test error scenarios

## Troubleshooting

### Common Issues

**"Network error" messages**
- Check PhishLabs API connectivity
- Verify API key and endpoints
- Review firewall/network policies

**Form submission failures**
- Verify CSRF token generation
- Check browser console for JavaScript errors
- Review server logs for detailed errors

**Rate limiting issues**
- Adjust `RateLimitPerMinute` setting
- Implement user-specific limits if needed
- Monitor IP-based abuse patterns

### Debug Mode
Enable debug logging in `appsettings.Development.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug"
    }
  }
}
```

## Support

For issues with:
- **Component functionality**: Check logs and configuration
- **PhishLabs API**: Contact PhishLabs support
- **Umbraco integration**: Review Umbraco documentation

## Security Considerations

1. **API Key Management**: Store in secure configuration (Key Vault, environment variables)
2. **HTTPS Only**: Ensure all communication uses TLS
3. **Input Sanitization**: All inputs are sanitized before processing
4. **Rate Limiting**: Implement appropriate limits for your use case
5. **Monitoring**: Set up alerts for unusual submission patterns