using FluentAssertions;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Domain.Models;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public class WeatherForecastAlreadyExistsExceptionTests
{
    [Fact]
    public void GivenValidValues_WhenWeatherForecastAlreadyExistsExceptionIsCreated_ThenAllPropertiesAreInitialized()
    {
        // Arrange
        var expectedLocation = Location.Create(10, 20);
        var expectedMessage = $"Weather forecast already exists for location: ({expectedLocation.Latitude}, {expectedLocation.Longitude})";

        // Act
        var exception = new WeatherForecastAlreadyExistsException(expectedLocation);

        // Assert
        exception.Location.Should().Be(expectedLocation);
        exception.Message.Should().Be(expectedMessage);
    }
}