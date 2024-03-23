using MediatR;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Notifications;

public sealed record LocationSearchedNotification(Location Location) : INotification;