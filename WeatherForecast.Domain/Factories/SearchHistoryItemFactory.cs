using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Providers;

namespace WeatherForecast.Domain.Factories;

public sealed class SearchHistoryItemFactory
{
    private readonly ITimeProvider _timeProvider;

    public SearchHistoryItemFactory(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public SearchHistoryItem Create(Location location)
    {
        return new SearchHistoryItem(
            Guid.Empty,
            location,
            _timeProvider.Now);
    }
}