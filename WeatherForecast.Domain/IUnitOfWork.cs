using WeatherForecast.Domain.Repositories;

namespace WeatherForecast.Domain;

public interface IUnitOfWork
{
    IWeatherForecastRepository WeatherForecasts { get; }
    
    ISearchHistoryItemRepository SearchHistory { get; }
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}