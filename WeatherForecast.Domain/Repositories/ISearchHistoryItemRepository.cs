using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Domain.Repositories;

public interface ISearchHistoryItemRepository
{
    Task<IReadOnlyCollection<SearchHistoryItem>> GetAllAsync(int limit, CancellationToken cancellationToken = default);
    
    Task<SearchHistoryItem?> FindByLocationAsync(Location location, CancellationToken cancellationToken = default);
    
    void Add(SearchHistoryItem searchHistoryItem);
}