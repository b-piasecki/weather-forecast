using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class ValidationExceptionTests
{
    [Fact]
    public void GivenValidValues_WhenValidationExceptionIsCreated_ThenAllPropertiesAreInitialized()
    {
        // Arrange
        const string expectedMessage = "Test exception message";
        
        // Act
        var exception = new ValidationException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }
}