using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecast.Domain;

namespace WeatherForecast.Application.Commands;

public sealed class DeleteWeatherForecastCommandHandler : IRequestHandler<DeleteWeatherForecastCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteWeatherForecastCommandHandler> _logger;

    public DeleteWeatherForecastCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteWeatherForecastCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteWeatherForecastCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting deletion of weather forecast for location: ({latitude}, {longitude})", 
            command.Location.Latitude,
            command.Location.Longitude);
        
        var weatherForecast = await _unitOfWork.WeatherForecasts.FindByLocationAsync(
            command.Location,
            cancellationToken);

        if (weatherForecast is null)
        {
            _logger.LogWarning(
                "Failed to delete weather forecast for location: ({latitude}, {longitude}). It does not exist.",
                command.Location.Latitude,
                command.Location.Longitude);
            return false;
        }
        
        _unitOfWork.WeatherForecasts.Remove(weatherForecast);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation(
            "Successfully deleted weather forecast for location: ({latitude}, {longitude})", 
            command.Location.Latitude,
            command.Location.Longitude);
        
        return true;
    }
}