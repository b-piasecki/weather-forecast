using MediatR;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Commands;

public sealed record AddWeatherForecastCommand(Location Location) : IRequest<bool>;