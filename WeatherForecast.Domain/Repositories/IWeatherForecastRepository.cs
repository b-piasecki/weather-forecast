using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Domain.Repositories;

public interface IWeatherForecastRepository
{
    Task<WeatherForecastData?> FindByLocationAsync(Location location, CancellationToken cancellationToken = default);
    
    void Add(WeatherForecastData weatherForecast);
    
    void Remove(WeatherForecastData weatherForecast);
}