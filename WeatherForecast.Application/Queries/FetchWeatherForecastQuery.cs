using MediatR;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Queries;

public sealed record FetchWeatherForecastQuery(Location Location, bool Newest) : IRequest<WeatherForecastData?>;