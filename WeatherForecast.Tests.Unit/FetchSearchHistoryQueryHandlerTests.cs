using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherForecast.Application.Queries;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Factories;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Providers;
using WeatherForecast.Domain.Repositories;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class FetchSearchHistoryQueryHandlerTests
{
    private readonly ISearchHistoryItemRepository _searchHistoryItemRepositoryMock;
    
    private readonly SearchHistoryItemFactory _searchHistoryItemFactory;
    private readonly FetchSearchHistoryQueryHandler _sut;
    
    public FetchSearchHistoryQueryHandlerTests()
    {
        _searchHistoryItemRepositoryMock = Substitute.For<ISearchHistoryItemRepository>();
        
        var timeProviderMock = Substitute.For<ITimeProvider>();
        _searchHistoryItemFactory = new SearchHistoryItemFactory(timeProviderMock);
        
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        unitOfWorkMock.SearchHistory.Returns(_searchHistoryItemRepositoryMock);
        
        var loggerMock = Substitute.For<ILogger<FetchSearchHistoryQueryHandler>>();
        _sut = new FetchSearchHistoryQueryHandler(unitOfWorkMock, loggerMock);
    }
    
    [Fact]
    public async Task GivenValidQuery_WhenQueryIsHandled_ThenSearchHistoryIsReturned()
    {
        // Arrange
        var query = new FetchSearchHistoryQuery(5);
        var cancellationToken = default(CancellationToken);
        
        var expectedSearchHistory = new List<SearchHistoryItem>
        {
            _searchHistoryItemFactory.Create(Location.Create(10, 20)),
            _searchHistoryItemFactory.Create(Location.Create(15, 25)),
        };
        _searchHistoryItemRepositoryMock
            .GetAllAsync(query.Limit, cancellationToken)
            .Returns(expectedSearchHistory);

        // Act
        var searchHistory = await _sut.Handle(query, cancellationToken);

        // Assert
        searchHistory.Should().BeEquivalentTo(expectedSearchHistory);
    }
}