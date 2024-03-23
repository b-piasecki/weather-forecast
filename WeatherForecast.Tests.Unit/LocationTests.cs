using FluentAssertions;
using WeatherForecast.Domain.Exceptions;
using WeatherForecast.Domain.Models;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class LocationTests
{
    [Theory]
    [InlineData(-90, 10)]
    [InlineData(90, 10)]
    [InlineData(10, -180)]
    [InlineData(10, 179.99)]
    public void GivenValidValues_WhenLocationIsCreated_ThenAllPropertiesAreInitialized(
        decimal expectedLatitude, 
        decimal expectedLongitude)
    {
        // Act
        var location = Location.Create(expectedLatitude, expectedLongitude);

        // Assert
        location.Latitude.Should().Be(expectedLatitude);
        location.Longitude.Should().Be(expectedLongitude);
    }
    
    [Theory]
    [InlineData(-90.01)]
    [InlineData(90.01)]
    public void GivenInvalidLatitude_WhenLocationIsCreated_ThenValidationExceptionIsThrown(decimal latitude)
    {
        // Arrange
        const decimal longitude = 10;
        
        // Act
        var action = () =>  Location.Create(latitude, longitude);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage("Latitude must be between -90 and 90");
    }
    
    [Theory]
    [InlineData(-180.01)]
    [InlineData(180)]
    public void GivenInvalidLongitude_WhenLocationIsCreated_ThenValidationExceptionIsThrown(decimal longitude)
    {
        // Arrange
        const decimal latitude = 10;
        
        // Act
        var action = () =>  Location.Create(latitude, longitude);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage("Longitude must be between -180 and 180");
    }
}