using Microsoft.EntityFrameworkCore;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Repositories;

namespace WeatherForecast.Infrastructure.Database.Repositories;

public sealed class SearchHistoryItemRepository : ISearchHistoryItemRepository
{
    private readonly DbSet<SearchHistoryItem> _searchHistory;

    public SearchHistoryItemRepository(DbSet<SearchHistoryItem> searchHistory)
    {
        _searchHistory = searchHistory;
    }

    public async Task<IReadOnlyCollection<SearchHistoryItem>> GetAllAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await _searchHistory
            .OrderByDescending(item => item.SearchedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<SearchHistoryItem?> FindByLocationAsync(
        Location location,
        CancellationToken cancellationToken = default)
    {
        return await _searchHistory.FirstOrDefaultAsync(
            searchHistoryItem => 
                searchHistoryItem.Location.Latitude == location.Latitude && 
                searchHistoryItem.Location.Longitude == location.Longitude, 
            cancellationToken);
    }

    public void Add(SearchHistoryItem searchHistoryItem)
    {
        _searchHistory.Add(searchHistoryItem);
    }
}