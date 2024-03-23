using WeatherForecast.Domain.Exceptions;

namespace WeatherForecast.Domain.Models;

public sealed class Location
{
    private static readonly (decimal Minimum, decimal Maximum) LatitudeRange = (-90, 90);
    private static readonly (decimal Minimum, decimal Maximum) LongitudeRange = (-180, 180);
    
    private Location(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public decimal Latitude { get; }
    
    public decimal Longitude { get; }
    
    public static Location Create(decimal latitude, decimal longitude)
    {
        if (latitude < LatitudeRange.Minimum || latitude > LatitudeRange.Maximum)
        {
            throw new ValidationException($"Latitude must be between {LatitudeRange.Minimum} and {LatitudeRange.Maximum}");
        }
        
        if (longitude < LongitudeRange.Minimum || longitude >= LongitudeRange.Maximum)
        {
            throw new ValidationException($"Longitude must be between {LongitudeRange.Minimum} and {LongitudeRange.Maximum}");
        }
        
        return new Location(latitude, longitude);
    }

    public Location Clone()
    {
        return Create(Latitude, Longitude);
    }
}