using System.ComponentModel.DataAnnotations;
using UmbracoWeb.Models;

namespace UmbracoWeb.Tests;

[TestFixture]
public class PhishLabsModelsTests
{
    [TestFixture]
    public class PhishLabsIncidentRequestTests
    {
        [Test]
        public void Url_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var request = new PhishLabsIncidentRequest
            {
                Url = string.Empty,
                Details = "Some details"
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.ErrorMessage!.Contains("URL is required")), Is.True);
        }

        [Test]
        public void Url_ValidUrl_ValidationSucceeds()
        {
            // Arrange
            var request = new PhishLabsIncidentRequest
            {
                Url = "https://example.com/suspicious",
                Details = "Some details"
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            var urlErrors = validationResults.Where(v => v.MemberNames.Contains("Url"));
            Assert.That(urlErrors, Is.Empty);
        }

        [Test]
        public void Url_InvalidUrl_ValidationFails()
        {
            // Arrange
            var request = new PhishLabsIncidentRequest
            {
                Url = "not-a-valid-url",
                Details = "Some details"
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.ErrorMessage!.Contains("valid URL")), Is.True);
        }

        [Test]
        public void Url_ExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var longUrl = "https://example.com/" + new string('a', 2100); // Exceeds 2048 limit
            var request = new PhishLabsIncidentRequest
            {
                Url = longUrl,
                Details = "Some details"
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.ErrorMessage!.Contains("2048 characters")), Is.True);
        }

        [Test]
        public void Details_ExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var longDetails = new string('a', 1001); // Exceeds 1000 limit
            var request = new PhishLabsIncidentRequest
            {
                Url = "https://example.com/suspicious",
                Details = longDetails
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.ErrorMessage!.Contains("1000 characters")), Is.True);
        }

        [Test]
        public void Details_NullOrEmpty_ValidationSucceeds()
        {
            // Arrange
            var request = new PhishLabsIncidentRequest
            {
                Url = "https://example.com/suspicious",
                Details = null
            };

            // Act
            var validationResults = ValidateModel(request);

            // Assert
            var detailsErrors = validationResults.Where(v => v.MemberNames.Contains("Details"));
            Assert.That(detailsErrors, Is.Empty);
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }

    [TestFixture]
    public class PhishLabsSettingsTests
    {
        [Test]
        public void SectionName_ReturnsCorrectValue()
        {
            // Act & Assert
            Assert.That(PhishLabsSettings.SectionName, Is.EqualTo("PhishLabs"));
        }

        [Test]
        public void ApiBaseUrl_Required_ValidationFails_WhenEmpty()
        {
            // Arrange
            var settings = new PhishLabsSettings
            {
                ApiBaseUrl = string.Empty,
                ApiKey = "test-key",
                ServicePath = "/incidents/test"
            };

            // Act
            var validationResults = ValidateModel(settings);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.MemberNames.Contains("ApiBaseUrl")), Is.True);
        }

        [Test]
        public void TimeoutSeconds_WithinRange_ValidationSucceeds()
        {
            // Arrange
            var settings = new PhishLabsSettings
            {
                ApiBaseUrl = "https://api.phishlabs.com",
                ApiKey = "test-key",
                ServicePath = "/incidents/test",
                TimeoutSeconds = 30
            };

            // Act
            var validationResults = ValidateModel(settings);

            // Assert
            var timeoutErrors = validationResults.Where(v => v.MemberNames.Contains("TimeoutSeconds"));
            Assert.That(timeoutErrors, Is.Empty);
        }

        [Test]
        public void TimeoutSeconds_OutOfRange_ValidationFails()
        {
            // Arrange
            var settings = new PhishLabsSettings
            {
                ApiBaseUrl = "https://api.phishlabs.com",
                ApiKey = "test-key",
                ServicePath = "/incidents/test",
                TimeoutSeconds = 500 // Outside range 5-300
            };

            // Act
            var validationResults = ValidateModel(settings);

            // Assert
            Assert.That(validationResults, Has.Count.GreaterThan(0));
            Assert.That(validationResults.Any(v => v.MemberNames.Contains("TimeoutSeconds")), Is.True);
        }

        [Test]
        public void MaxRetries_WithinRange_ValidationSucceeds()
        {
            // Arrange
            var settings = new PhishLabsSettings
            {
                ApiBaseUrl = "https://api.phishlabs.com",
                ApiKey = "test-key",
                ServicePath = "/incidents/test",
                MaxRetries = 3
            };

            // Act
            var validationResults = ValidateModel(settings);

            // Assert
            var retriesErrors = validationResults.Where(v => v.MemberNames.Contains("MaxRetries"));
            Assert.That(retriesErrors, Is.Empty);
        }

        [Test]
        public void RateLimitPerMinute_WithinRange_ValidationSucceeds()
        {
            // Arrange
            var settings = new PhishLabsSettings
            {
                ApiBaseUrl = "https://api.phishlabs.com",
                ApiKey = "test-key",
                ServicePath = "/incidents/test",
                RateLimitPerMinute = 10
            };

            // Act
            var validationResults = ValidateModel(settings);

            // Assert
            var rateLimitErrors = validationResults.Where(v => v.MemberNames.Contains("RateLimitPerMinute"));
            Assert.That(rateLimitErrors, Is.Empty);
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}