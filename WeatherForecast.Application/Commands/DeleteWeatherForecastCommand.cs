using MediatR;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Commands;

public sealed record DeleteWeatherForecastCommand(Location Location) : IRequest<bool>;