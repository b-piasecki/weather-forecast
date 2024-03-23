namespace WeatherForecast.Api.Contracts.Responses;

public sealed record GetWeatherForecastResponse(
    decimal Latitude,
    decimal Longitude,
    DateTimeOffset Time,
    decimal Temperature,
    decimal Humidity,
    decimal WindSpeed);