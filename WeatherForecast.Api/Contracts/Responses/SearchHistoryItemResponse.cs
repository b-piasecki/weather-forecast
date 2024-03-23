namespace WeatherForecast.Api.Contracts.Responses;

public sealed record SearchHistoryItemResponse(
    decimal Latitude,
    decimal Longitude,
    DateTimeOffset SearchedAt);