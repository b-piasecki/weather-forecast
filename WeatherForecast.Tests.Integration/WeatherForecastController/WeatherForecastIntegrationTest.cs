using System.Net.Http.Json;
using WeatherForecast.Api.Contracts.Requests;

namespace WeatherForecast.Tests.Integration.WeatherForecastController;

public abstract class WeatherForecastIntegrationTest : IntegrationTest
{
    protected const string BaseUrl = "WeatherForecast";
    
    protected WeatherForecastIntegrationTest(ApiFactory factory) 
        : base(factory)
    {
    }
    
    protected async Task SearchWeatherForecastAsync(decimal latitude, decimal longitude)
    {
        var client = CreateClient();
        var url = $"{BaseUrl}?latitude={latitude}&longitude={longitude}";

        var responseMessage = await client.GetAsync(url);
        responseMessage.EnsureSuccessStatusCode();
    }
    
    protected async Task AddWeatherForecastAsync(decimal latitude, decimal longitude)
    {
        var client = CreateClient();
        
        var request = new AddWeatherForecastRequest(latitude, longitude);
        var content = JsonContent.Create(request);
        
        var responseMessage = await client.PostAsync(BaseUrl, content);
        responseMessage.EnsureSuccessStatusCode();
    }
}