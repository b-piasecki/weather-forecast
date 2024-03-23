using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using WeatherForecast.Api.Contracts.Requests;
using Xunit;

namespace WeatherForecast.Tests.Integration.WeatherForecastController;

public sealed class AddWeatherForecastIntegrationTests : WeatherForecastIntegrationTest
{
    public AddWeatherForecastIntegrationTests(ApiFactory factory) 
        : base(factory)
    {
    }
    
    [Fact]
    public async Task GivenValidLocationAndNoExistingWeatherForecast_WhenWeatherForecastIsAdded_ThenNoContentIsReturned()
    {
        // Arrange
        var client = CreateClient();
        var content = JsonContent.Create(new AddWeatherForecastRequest(10, 20));

        // Act
        var responseMessage = await client.PostAsync(BaseUrl, content);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task GivenValidLocationAndExistingWeatherForecast_WhenWeatherForecastIsAdded_ThenConflictIsReturned()
    {
        // Arrange
        const decimal latitude = 15;
        const decimal longitude = 25;

        await AddWeatherForecastAsync(latitude, longitude);
        
        var client = CreateClient();
        var content = JsonContent.Create(new AddWeatherForecastRequest(latitude, longitude));

        // Act
        var responseMessage = await client.PostAsync(BaseUrl, content);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    public async Task GivenInvalidLocation_WhenWeatherForecastIsAdded_ThenBadRequestIsReturned(
        decimal latitude,
        decimal longitude)
    {
        // Arrange
        var client = CreateClient();
        var content = JsonContent.Create(new AddWeatherForecastRequest(latitude, longitude));

        // Act
        var responseMessage = await client.PostAsync(BaseUrl, content);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}