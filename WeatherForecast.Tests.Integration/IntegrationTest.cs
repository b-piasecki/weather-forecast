using System.Net.Http.Json;
using Xunit;

namespace WeatherForecast.Tests.Integration;

public abstract class IntegrationTest : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    
    protected IntegrationTest(ApiFactory factory)
    {
        _factory = factory;
    }
    
    protected HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }
    
    protected async Task<T> ReadResultBodyAsync<T>(HttpResponseMessage responseMessage)
    {
        var response = await responseMessage.Content.ReadFromJsonAsync<T>();

        if (response is null)
        {
            throw new ArgumentException("Failed to read response body.", nameof(responseMessage));
        }

        return response;
    }
}