using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Clients;
using WeatherForecast.Application.Notifications;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Queries;

public sealed class FetchWeatherForecastQueryHandler : IRequestHandler<FetchWeatherForecastQuery, WeatherForecastData?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWeatherForecastClient _weatherForecastClient;
    private readonly IMediator _mediator;
    private readonly ILogger<FetchWeatherForecastQueryHandler> _logger;

    public FetchWeatherForecastQueryHandler(
        IUnitOfWork unitOfWork,
        IWeatherForecastClient weatherForecastClient,
        IMediator mediator,
        ILogger<FetchWeatherForecastQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _weatherForecastClient = weatherForecastClient;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<WeatherForecastData?> Handle(FetchWeatherForecastQuery query, CancellationToken cancellationToken)
    {
        if (query.Newest)
        {
            _logger.LogInformation(
                "Attempting to fetch newest weather forecast for location: ({latitude}, {longitude})",
                query.Location.Latitude,
                query.Location.Longitude);
        }
        else
        {
            _logger.LogInformation(
                "Attempting to fetch weather forecast for location: ({latitude}, {longitude})",
                query.Location.Latitude,
                query.Location.Longitude);
        }
        
        try
        {
            var localWeatherForecast = await FetchLocalWeatherForecastAsync(query.Location, cancellationToken);

            if (!query.Newest)
            {
                if (localWeatherForecast is not null)
                {
                    _logger.LogInformation(
                        "Found local weather forecast for location: ({latitude}, {longitude})",
                        query.Location.Latitude,
                        query.Location.Longitude);
                    return localWeatherForecast;
                }
                
                _logger.LogInformation(
                    "No local weather forecast found. Fetching from client for location: ({latitude}, {longitude})",
                    query.Location.Latitude,
                    query.Location.Longitude);
            }
            
            var clientWeatherForecast = await FetchClientWeatherForecastAsync(query.Location, cancellationToken);

            if (clientWeatherForecast is null)
            {
                _logger.LogWarning(
                    "Failed to fetch weather forecast from client for location: ({latitude}, {longitude})",
                    query.Location.Latitude,
                    query.Location.Longitude);
                return null;
            }
            
            _logger.LogInformation(
                "Found client weather forecast for location: ({latitude}, {longitude}). Adding to local store.",
                query.Location.Latitude,
                query.Location.Longitude);
            
            await StoreWeatherForecastAsync(clientWeatherForecast, localWeatherForecast, cancellationToken);
            
            _logger.LogInformation(
                "Successfully added weather forecast for location: ({latitude}, {longitude})",
                query.Location.Latitude,
                query.Location.Longitude);
            return clientWeatherForecast;
        }
        catch (SqlException ex)
        {
            _logger.LogError(
                ex,
                "Database error while fetching weather forecast for location: ({latitude}, {longitude}). Attempting to fetch from client.",
                query.Location.Latitude,
                query.Location.Longitude);

            var clientWeatherForecast = await FetchClientWeatherForecastAsync(
                query.Location,
                cancellationToken);
            return clientWeatherForecast;
        }
        finally
        {
            var notification = new LocationSearchedNotification(query.Location);
            await _mediator.Publish(notification, cancellationToken);
        }
    }
    
    private async Task<WeatherForecastData?> FetchLocalWeatherForecastAsync(
        Location location,
        CancellationToken cancellationToken)
    {
        var weatherForecast = await _unitOfWork.WeatherForecasts.FindByLocationAsync(
            location,
            cancellationToken);
        return weatherForecast;
    }
    
    private async Task<WeatherForecastData?> FetchClientWeatherForecastAsync(
        Location location,
        CancellationToken cancellationToken)
    {
        var weatherForecast = await _weatherForecastClient.FetchWeatherForecastAsync(location, cancellationToken);
        return weatherForecast;
    }
    
    private async Task StoreWeatherForecastAsync(
        WeatherForecastData clientWeatherForecast,
        WeatherForecastData? localWeatherForecast,
        CancellationToken cancellationToken)
    {
        if (localWeatherForecast is null)
        {
            _unitOfWork.WeatherForecasts.Add(clientWeatherForecast);
        }
        else
        {
            localWeatherForecast.Time = clientWeatherForecast.Time;
            localWeatherForecast.Temperature = clientWeatherForecast.Temperature;
            localWeatherForecast.Humidity = clientWeatherForecast.Humidity;
            localWeatherForecast.WindSpeed = clientWeatherForecast.WindSpeed;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}