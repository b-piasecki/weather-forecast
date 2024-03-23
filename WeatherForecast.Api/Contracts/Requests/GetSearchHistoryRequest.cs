using Microsoft.AspNetCore.Mvc;

namespace WeatherForecast.Api.Contracts.Requests;

public sealed record GetSearchHistoryRequest(
    [property: FromQuery]int Limit = 10);