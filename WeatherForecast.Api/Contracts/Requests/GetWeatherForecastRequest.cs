using Microsoft.AspNetCore.Mvc;

namespace WeatherForecast.Api.Contracts.Requests;

public sealed record GetWeatherForecastRequest(
    [property: FromQuery]decimal Latitude, 
    [property: FromQuery]decimal Longitude,
    [property: FromQuery]bool Newest);