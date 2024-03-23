using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Clients;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Domain;

namespace WeatherForecast.Application.Commands;

public sealed class AddWeatherForecastCommandHandler : IRequestHandler<AddWeatherForecastCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWeatherForecastClient _weatherForecastClient;
    private readonly ILogger<AddWeatherForecastCommandHandler> _logger;

    public AddWeatherForecastCommandHandler(
        IUnitOfWork unitOfWork,
        IWeatherForecastClient weatherForecastClient,
        ILogger<AddWeatherForecastCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _weatherForecastClient = weatherForecastClient;
        _logger = logger;
    }
    
    public async Task<bool> Handle(
        AddWeatherForecastCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Attempting to add weather forecast for location: ({latitude}, {longitude})",
            command.Location.Latitude,
            command.Location.Longitude);
        
        var existingWeatherForecast = await _unitOfWork.WeatherForecasts.FindByLocationAsync(
            command.Location,
            cancellationToken);
        
        if (existingWeatherForecast is not null)
        {
            _logger.LogWarning(
                "Failed to add weather forecast for location: ({latitude}, {longitude}). It already exists.",
                command.Location.Latitude,
                command.Location.Longitude);
            throw new WeatherForecastAlreadyExistsException(command.Location);
        }
        
        var clientWeatherForecast = await _weatherForecastClient.FetchWeatherForecastAsync(
            command.Location,
            cancellationToken);

        if (clientWeatherForecast is null)
        {
            _logger.LogWarning(
                "Failed to add weather forecast for location: ({latitude}, {longitude}). No data to add.",
                command.Location.Latitude,
                command.Location.Longitude);
            return false;
        }
        
        _unitOfWork.WeatherForecasts.Add(clientWeatherForecast);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Successfully added weather forecast for location: ({latitude}, {longitude})", 
            command.Location.Latitude,
            command.Location.Longitude);
        
        return true;
    }
}