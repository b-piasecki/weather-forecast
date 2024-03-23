using System.Net.Http.Json;
using WeatherForecast.Application.Clients;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Clients.OpenMeteo.Responses;

namespace WeatherForecast.Infrastructure.Clients.OpenMeteo;

public sealed class OpenMeteoWeatherForecastClient : IWeatherForecastClient
{
    private const string GetWeatherForecastUrlFormat =
        "/v1/forecast?latitude={0}&longitude={1}&current=temperature_2m,relative_humidity_2m,wind_speed_10m";

    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherForecastClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecastData?> FetchWeatherForecastAsync(
        Location location,
        CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync(location, cancellationToken);

        if (response is null)
        {
            return null;
        }

        var weatherForecast = ConstructWeatherForecast(location, response);
        return weatherForecast;
    }

    private async Task<WeatherForecastResponse?> GetResponseAsync(
        Location location,
        CancellationToken cancellationToken)
    {
        var url = GetWeatherForecastUrl(location);
        var response = await _httpClient.GetFromJsonAsync<WeatherForecastResponse>(url, cancellationToken);

        return response;
    }

    private string GetWeatherForecastUrl(Location location)
    {
        return string.Format(GetWeatherForecastUrlFormat, location.Latitude, location.Longitude);
    }

    private WeatherForecastData ConstructWeatherForecast(Location location, WeatherForecastResponse response)
    {
        return WeatherForecastData.Create(
            location,
            response.CurrentWeatherForecast.Time,
            response.CurrentWeatherForecast.Temperature,
            response.CurrentWeatherForecast.Humidity,
            response.CurrentWeatherForecast.WindSpeed);
    }
}