using Microsoft.EntityFrameworkCore;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Repositories;

namespace WeatherForecast.Infrastructure.Database.Repositories;

public sealed class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly DbSet<WeatherForecastData> _weatherForecasts;

    public WeatherForecastRepository(DbSet<WeatherForecastData> weatherForecasts)
    {
        _weatherForecasts = weatherForecasts;
    }

    public async Task<WeatherForecastData?> FindByLocationAsync(
        Location location,
        CancellationToken cancellationToken = default)
    {
        return await _weatherForecasts.FirstOrDefaultAsync(
            weatherForecast => 
                weatherForecast.Location.Latitude == location.Latitude && 
                weatherForecast.Location.Longitude == location.Longitude, 
            cancellationToken);
    }

    public void Add(WeatherForecastData weatherForecast)
    {
        _weatherForecasts.Add(weatherForecast);
    }

    public void Remove(WeatherForecastData weatherForecast)
    {
        _weatherForecasts.Remove(weatherForecast);
    }
}