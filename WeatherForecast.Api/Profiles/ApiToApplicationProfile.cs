using AutoMapper;
using WeatherForecast.Api.Contracts.Requests;
using WeatherForecast.Application.Commands;
using WeatherForecast.Application.Queries;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Api.Profiles;

public sealed class ApiToApplicationProfile : Profile
{
    public ApiToApplicationProfile()
    {
        CreateMap<GetWeatherForecastRequest, FetchWeatherForecastQuery>()
            .ConstructUsing(request => 
                new FetchWeatherForecastQuery(Location.Create(request.Latitude, request.Longitude), request.Newest));
        
        CreateMap<AddWeatherForecastRequest, AddWeatherForecastCommand>()
            .ConstructUsing(request => 
                new AddWeatherForecastCommand(Location.Create(request.Latitude, request.Longitude)));
        
        CreateMap<DeleteWeatherForecastRequest, DeleteWeatherForecastCommand>()
            .ConstructUsing(request => 
                new DeleteWeatherForecastCommand(Location.Create(request.Latitude, request.Longitude)));
        
        CreateMap<GetSearchHistoryRequest, FetchSearchHistoryQuery>()
            .ConstructUsing(request => new FetchSearchHistoryQuery(request.Limit));
    }
}