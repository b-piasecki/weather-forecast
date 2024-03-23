using System.Text.Json.Serialization;

namespace WeatherForecast.Infrastructure.Clients.OpenMeteo.Responses;

public sealed record WeatherForecastResponse([property: JsonPropertyName("current")]CurrentWeatherForecastResponse CurrentWeatherForecast);