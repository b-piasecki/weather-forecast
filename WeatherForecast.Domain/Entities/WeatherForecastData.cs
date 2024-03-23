using WeatherForecast.Domain.Models;

namespace WeatherForecast.Domain.Entities;

public sealed class WeatherForecastData
{
    private WeatherForecastData(
        Guid id,
        DateTimeOffset time,
        decimal temperature,
        decimal humidity,
        decimal windSpeed)
    {
        Id = id;
        Location = default!;
        Time = time;
        Temperature = temperature;
        Humidity = humidity;
        WindSpeed = windSpeed;
    }
    
    private WeatherForecastData(
        Guid id,
        Location location,
        DateTimeOffset time,
        decimal temperature,
        decimal humidity,
        decimal windSpeed)
        : this(id, time, temperature, humidity, windSpeed)
    {
        Location = location;
    }

    public Guid Id { get; }

    public Location Location { get; }

    public DateTimeOffset Time { get; set; }

    public decimal Temperature { get; set; }

    public decimal Humidity { get; set; }

    public decimal WindSpeed { get; set; }

    public static WeatherForecastData Create(
        Location location,
        DateTimeOffset time,
        decimal temperature,
        decimal humidity,
        decimal windSpeed)
    {
        return new WeatherForecastData(
            Guid.Empty,
            location,
            time,
            temperature,
            humidity,
            windSpeed);
    }
}