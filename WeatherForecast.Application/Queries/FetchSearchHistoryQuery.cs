using MediatR;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Queries;

public sealed record FetchSearchHistoryQuery(int Limit) : IRequest<IReadOnlyCollection<SearchHistoryItem>>;