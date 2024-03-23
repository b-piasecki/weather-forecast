using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.Clients.OpenMeteo.Responses;

public sealed record CurrentWeatherForecastResponse(
    DateTimeOffset Time, 
    [property: JsonPropertyName("temperature_2m")]decimal Temperature,
    [property: JsonPropertyName("relative_humidity_2m")]decimal Humidity,
    [property: JsonPropertyName("wind_speed_10m")]decimal WindSpeed);