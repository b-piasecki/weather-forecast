using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Exceptions;

public sealed class WeatherForecastAlreadyExistsException : InvalidOperationException
{
    public WeatherForecastAlreadyExistsException(Location location)
        : base($"Weather forecast already exists for location: ({location.Latitude}, {location.Longitude})")
    {
        Location = location;
    }
    
    public Location Location { get; }
}