using WeatherForecast.Domain;
using WeatherForecast.Domain.Repositories;
using WeatherForecast.Infrastructure.Database;

namespace WeatherForecast.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly WeatherForecastContext _context;

    public UnitOfWork(
        IWeatherForecastRepository weatherForecasts,
        ISearchHistoryItemRepository searchHistory,
        WeatherForecastContext context)
    {
        _context = context;
        WeatherForecasts = weatherForecasts;
        SearchHistory = searchHistory;
    }

    public IWeatherForecastRepository WeatherForecasts { get; }
    
    public ISearchHistoryItemRepository SearchHistory { get; }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}