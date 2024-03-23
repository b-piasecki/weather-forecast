using AutoMapper;
using WeatherForecast.Api.Contracts.Responses;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Api.Profiles;

public sealed class DomainToApiProfile : Profile
{
    public DomainToApiProfile()
    {
        CreateMap<WeatherForecastData, GetWeatherForecastResponse>()
            .ConstructUsing(weatherForecast => 
                new GetWeatherForecastResponse(
                    weatherForecast.Location.Latitude, 
                    weatherForecast.Location.Longitude, 
                    weatherForecast.Time, 
                    weatherForecast.Temperature, 
                    weatherForecast.Humidity, 
                    weatherForecast.WindSpeed));
        
        CreateMap<SearchHistoryItem, SearchHistoryItemResponse>()
            .ConstructUsing(searchHistoryItem => 
                new SearchHistoryItemResponse(
                    searchHistoryItem.Location.Latitude, 
                    searchHistoryItem.Location.Longitude, 
                    searchHistoryItem.SearchedAt));
    }
}