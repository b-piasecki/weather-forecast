using System.Net;
using FluentAssertions;
using Xunit;

namespace WeatherForecast.Tests.Integration.WeatherForecastController;

public class DeleteWeatherForecastIntegrationTests : WeatherForecastIntegrationTest
{
    public DeleteWeatherForecastIntegrationTests(ApiFactory factory) 
        : base(factory)
    {
    }
    
    [Fact]
    public async Task GivenValidLocationAndExistingWeatherForecast_WhenWeatherForecastIsDeleted_ThenNoContentIsReturned()
    {
        // Arrange
        const decimal latitude = 15;
        const decimal longitude = 25;

        await AddWeatherForecastAsync(latitude, longitude);
        
        var client = CreateClient();
        var url = GetUrl(latitude, longitude);

        // Act
        var responseMessage = await client.DeleteAsync(url);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task GivenValidLocationAndNoExistingWeatherForecast_WhenWeatherForecastIsDeleted_ThenNotFoundIsReturned()
    {
        // Arrange
        const decimal latitude = 15;
        const decimal longitude = 25;
        
        var client = CreateClient();
        var url = GetUrl(latitude, longitude);

        // Act
        var responseMessage = await client.DeleteAsync(url);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    public async Task GivenInvalidLocation_WhenWeatherForecastIsDeleted_ThenBadRequestIsReturned(
        decimal latitude,
        decimal longitude)
    {
        // Arrange
        var client = CreateClient();
        var url = GetUrl(latitude, longitude);

        // Act
        var responseMessage = await client.DeleteAsync(url);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    private string GetUrl(decimal latitude, decimal longitude)
    {
        return $"{BaseUrl}?latitude={latitude}&longitude={longitude}";
    }
}