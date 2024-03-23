using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Factories;
using WeatherForecast.Domain.Providers;

namespace WeatherForecast.Application.Notifications;

public sealed class SaveSearchedLocationNotificationHandler : INotificationHandler<LocationSearchedNotification>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeProvider _timeProvider;
    private readonly SearchHistoryItemFactory _searchHistoryItemFactory;
    private readonly ILogger<SaveSearchedLocationNotificationHandler> _logger;

    public SaveSearchedLocationNotificationHandler(
        IUnitOfWork unitOfWork,
        ITimeProvider timeProvider,
        SearchHistoryItemFactory searchHistoryItemFactory,
        ILogger<SaveSearchedLocationNotificationHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
        _searchHistoryItemFactory = searchHistoryItemFactory;
        _logger = logger;
    }

    public async Task Handle(LocationSearchedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Attempting to save searched location: ({latitude}, {longitude})",
            notification.Location.Latitude,
            notification.Location.Longitude);
        
        try
        {
            var searchHistoryItem = await _unitOfWork.SearchHistory.FindByLocationAsync(
                notification.Location,
                cancellationToken);
        
            if (searchHistoryItem is null)
            {
                searchHistoryItem = _searchHistoryItemFactory.Create(notification.Location);
                _unitOfWork.SearchHistory.Add(searchHistoryItem);
            }
            else
            {
                _logger.LogInformation(
                    "Location ({latitude}, {longitude}) has been already searched. Updating searched at time.",
                    notification.Location.Latitude,
                    notification.Location.Longitude);
                searchHistoryItem.SearchedAt = _timeProvider.Now;
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Searched location ({latitude}, {longitude}) saved successfully",
                notification.Location.Latitude,
                notification.Location.Longitude);
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                "Database error while saving searched location: ({latitude}, {longitude})",
                notification.Location.Latitude,
                notification.Location.Longitude);
        }
    }
}