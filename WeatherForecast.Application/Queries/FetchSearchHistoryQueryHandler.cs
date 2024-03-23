using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Queries;

public sealed class FetchSearchHistoryQueryHandler 
    : IRequestHandler<FetchSearchHistoryQuery, IReadOnlyCollection<SearchHistoryItem>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FetchSearchHistoryQueryHandler> _logger;

    public FetchSearchHistoryQueryHandler(IUnitOfWork unitOfWork, ILogger<FetchSearchHistoryQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<IReadOnlyCollection<SearchHistoryItem>> Handle(
        FetchSearchHistoryQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to fetch last {limit} search history items", query.Limit);
        
        var searchHistory = await _unitOfWork.SearchHistory.GetAllAsync(
            query.Limit,
            cancellationToken);
        
        if (searchHistory.Count == 0)
        {
            _logger.LogWarning("No search history items were found");
        }
        else
        {
            _logger.LogInformation(
                "{Count} search history items were successfully retrieved", 
                searchHistory.Count);
        }
        
        return searchHistory;
    }
}