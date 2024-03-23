using System.Net;
using FluentAssertions;
using WeatherForecast.Api.Contracts.Responses;
using Xunit;

namespace WeatherForecast.Tests.Integration.WeatherForecastController;

public sealed class GetWeatherForecastIntegrationTests : WeatherForecastIntegrationTest
{
    public GetWeatherForecastIntegrationTests(ApiFactory factory) 
        : base(factory)
    {
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GivenValidLocation_WhenWeatherForecastIsSearched_ThenWeatherForecastIsReturned(bool newest)
    {
        // Arrange
        const decimal expectedLatitude = 10;
        const decimal expectedLongitude = 20;
        
        var client = CreateClient();
        var url = GetUrl(expectedLatitude, expectedLongitude, newest);

        // Act
        var responseMessage = await client.GetAsync(url);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await ReadResultBodyAsync<GetWeatherForecastResponse>(responseMessage);
        response.Latitude.Should().Be(expectedLatitude);
        response.Longitude.Should().Be(expectedLongitude);
    }
    
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(0, 181)]
    public async Task GivenInvalidLocation_WhenWeatherForecastIsSearched_ThenBadRequestIsReturned(
        decimal latitude,
        decimal longitude)
    {
        // Arrange
        var client = CreateClient();
        var url = GetUrl(latitude, longitude);

        // Act
        var responseMessage = await client.GetAsync(url);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private string GetUrl(decimal latitude, decimal longitude, bool newest = false)
    {
        return $"{BaseUrl}?latitude={latitude}&longitude={longitude}&newest={newest}";
    }
}