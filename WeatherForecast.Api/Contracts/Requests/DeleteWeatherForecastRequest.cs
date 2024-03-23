using Microsoft.AspNetCore.Mvc;

namespace WeatherForecast.Api.Contracts.Requests;

public sealed record DeleteWeatherForecastRequest(
    [property: FromQuery]decimal Latitude, 
    [property: FromQuery]decimal Longitude);