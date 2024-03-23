using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WeatherForecast.Api.Contracts.Requests;
using WeatherForecast.Api.Contracts.Responses;
using WeatherForecast.Application.Commands;
using WeatherForecast.Application.Queries;

namespace WeatherForecast.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public WeatherForecastController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Retrieves weather forecast for a specific location")]
    [SwaggerResponse(200, "Returns the weather forecast", typeof(GetWeatherForecastResponse))]
    [SwaggerResponse(404, "Weather forecast not found for the specified location")]
    public async Task<IActionResult> GetWeatherForecast(
        [FromQuery]GetWeatherForecastRequest request,
        CancellationToken cancellationToken)
    {
        var query = _mapper.Map<FetchWeatherForecastQuery>(request);
        var weatherForecast = await _mediator.Send(query, cancellationToken);
    
        return weatherForecast switch
        {
            null => NotFound($"Weather forecast for location ({request.Latitude}, {request.Longitude})' not found."),
            _ => Ok(_mapper.Map<GetWeatherForecastResponse>(weatherForecast))
        };
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Adds a new weather forecast")]
    [SwaggerResponse(204, "Weather forecast added successfully")]
    [SwaggerResponse(400, "Failed to add weather forecast")]
    [SwaggerResponse(409, "Weather forecast already exists for the specified location")]
    public async Task<IActionResult> AddWeatherForecast(
        [FromBody] AddWeatherForecastRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<AddWeatherForecastCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        
        return result switch
        {
            false => BadRequest("Failed to add weather forecast."),
            true => NoContent()
        };
    }
    
    [HttpDelete]
    [SwaggerOperation(Summary = "Deletes a weather forecast")]
    [SwaggerResponse(204, "Weather forecast deleted successfully")]
    [SwaggerResponse(404, "Weather forecast not found for the specified location")]
    public async Task<IActionResult> DeleteWeatherForecast(
        [FromQuery]DeleteWeatherForecastRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<DeleteWeatherForecastCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        
        return result switch
        {
            false => NotFound($"Weather forecast for location ({request.Latitude}, {request.Longitude})' not found."),
            true => NoContent()
        };
    }
    
    [HttpGet("history")]
    [SwaggerOperation(Summary = "Retrieves the search history for weather forecasts")]
    [SwaggerResponse(200, "Returns the search history", typeof(IReadOnlyCollection<SearchHistoryItemResponse>))]
    public async Task<IActionResult> GetSearchHistory(
        [FromQuery]GetSearchHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var query = _mapper.Map<FetchSearchHistoryQuery>(request);
        var searchHistory = await _mediator.Send(query, cancellationToken);

        return Ok(_mapper.Map<IReadOnlyCollection<SearchHistoryItemResponse>>(searchHistory));
    }
}