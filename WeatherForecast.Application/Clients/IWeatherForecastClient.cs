using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Clients;

public interface IWeatherForecastClient
{
    Task<WeatherForecastData?> FetchWeatherForecastAsync(
        Location location,
        CancellationToken cancellationToken = default);
}