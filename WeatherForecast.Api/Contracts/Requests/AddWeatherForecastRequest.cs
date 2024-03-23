namespace WeatherForecast.Api.Contracts.Requests;

public sealed record AddWeatherForecastRequest(decimal Latitude, decimal Longitude);