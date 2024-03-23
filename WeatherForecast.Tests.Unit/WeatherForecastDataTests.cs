using FluentAssertions;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class WeatherForecastDataTests
{
    [Fact]
    public void GivenValidValues_WhenWeatherForecastIsCreated_ThenAllPropertiesAreInitialized()
    {
        // Arrange
        var expectedLocation = Location.Create(10, 20);
        var expectedTime = new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero);
        const decimal expectedTemperature = 30;
        const decimal expectedHumidity = 40;
        const decimal expectedWindSpeed = 50;

        // Act
        var weatherForecast = WeatherForecastData.Create(
            expectedLocation,
            expectedTime,
            expectedTemperature,
            expectedHumidity,
            expectedWindSpeed);

        // Assert
        weatherForecast.Id.Should().BeEmpty();
        weatherForecast.Location.Should().Be(expectedLocation);
        weatherForecast.Time.Should().Be(expectedTime);
        weatherForecast.Temperature.Should().Be(expectedTemperature);
        weatherForecast.Humidity.Should().Be(expectedHumidity);
        weatherForecast.WindSpeed.Should().Be(expectedWindSpeed);
    }
}