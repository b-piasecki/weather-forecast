using System.Net;
using FluentAssertions;
using WeatherForecast.Api.Contracts.Responses;
using Xunit;

namespace WeatherForecast.Tests.Integration.WeatherForecastController;

public class GetSearchHistoryWeatherForecastIntegrationTests
{
    public class GivenNoSearchHistory : WeatherForecastIntegrationTest
    {
        public GivenNoSearchHistory(ApiFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task WhenSearchHistoryIsFetched_ThenEmptyResponseIsReturned()
        {
            // Arrange
            var client = CreateClient();
            const string url = $"{BaseUrl}/history";

            // Act
            var responseMessage = await client.GetAsync(url);

            // Assert
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
        
            var response = await ReadResultBodyAsync<IReadOnlyCollection<SearchHistoryItemResponse>>(responseMessage);
            response.Should().BeEmpty();
        }
    }
    
    public class GivenExistingSearchHistory : WeatherForecastIntegrationTest
    {
        public GivenExistingSearchHistory(ApiFactory factory) : base(factory)
        {
        }
        
        [Fact]
        public async Task WhenSearchHistoryIsFetched_ThenNotEmptyCollectionIsReturned()
        {
            // Arrange
            const decimal expectedLatitude = 10;
            const decimal expectedLongitude = 20;
    
            await SearchWeatherForecastAsync(expectedLatitude, expectedLongitude);
            
            var client = CreateClient();
            var url = GetUrl();
    
            // Act
            var responseMessage = await client.GetAsync(url);
    
            // Assert
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var response = await ReadResultBodyAsync<IReadOnlyCollection<SearchHistoryItemResponse>>(responseMessage);
            response.Should().NotBeEmpty();
            response.Should().Contain(
                item => item.Latitude == expectedLatitude && item.Longitude == expectedLongitude);
        }
        
        [Fact]
        public async Task WhenLimitedSearchHistoryIsFetched_ThenNotEmptyCollectionWithLimitedNumberOfItemsIsReturned()
        {
            // Arrange
            var locations = new List<(decimal, decimal)>()
            {
                new(10, 20),
                new(12.5m, 22.5m),
                new(15, 25),
                new(17.5m, 27.5m),
            };
    
            foreach (var (latitude, longitude) in locations)
            {
                await SearchWeatherForecastAsync(latitude, longitude);
            }
    
            var client = CreateClient();
            
            const int expectedLimit = 2;
            var url = GetUrl(expectedLimit);
    
            // Act
            var responseMessage = await client.GetAsync(url);
    
            // Assert
            responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var response = await ReadResultBodyAsync<IReadOnlyCollection<SearchHistoryItemResponse>>(responseMessage);
            response.Should().NotBeEmpty();
            response.Should().HaveCount(expectedLimit);
        }
        
        private string GetUrl(int limit = 5)
        {
            return $"{BaseUrl}/history?limit={limit}";
        }
    }
}