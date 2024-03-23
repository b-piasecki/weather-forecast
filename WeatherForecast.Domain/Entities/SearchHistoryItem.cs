using WeatherForecast.Domain.Models;

namespace WeatherForecast.Domain.Entities;

public sealed class SearchHistoryItem
{
    private SearchHistoryItem(
        Guid id,
        DateTimeOffset searchedAt)
    {
        Id = id;
        Location = default!;
        SearchedAt = searchedAt;
    }
    
    internal SearchHistoryItem(
        Guid id,
        Location location,
        DateTimeOffset searchedAt)
        : this(id, searchedAt)
    {
        Location = location;
    }
    
    public Guid Id { get; }

    public Location Location { get; }
    
    public DateTimeOffset SearchedAt { get; set; }
}