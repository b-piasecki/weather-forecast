using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherForecast.Application.Notifications;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Factories;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Providers;
using WeatherForecast.Domain.Repositories;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class SaveSearchedLocationNotificationHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ISearchHistoryItemRepository _searchHistoryItemRepositoryMock;
    private readonly ITimeProvider _timeProviderMock;
    private readonly SearchHistoryItemFactory _searchHistoryItemFactory;
    
    private readonly SaveSearchedLocationNotificationHandler _sut;
    
    public SaveSearchedLocationNotificationHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        _searchHistoryItemRepositoryMock = Substitute.For<ISearchHistoryItemRepository>();
        _unitOfWorkMock.SearchHistory.Returns(_searchHistoryItemRepositoryMock);
        
        _timeProviderMock = Substitute.For<ITimeProvider>();
        _searchHistoryItemFactory = new SearchHistoryItemFactory(_timeProviderMock);
        
        var loggerMock = Substitute.For<ILogger<SaveSearchedLocationNotificationHandler>>();
        
        _sut = new SaveSearchedLocationNotificationHandler(
            _unitOfWorkMock,
            _timeProviderMock,
            _searchHistoryItemFactory,
            loggerMock);
    }
    
    [Fact]
    public async Task GivenValidNotification_WhenNotificationIsHandledAndLocationWasNotSearchedBefore_ThenSearchedLocationIsSaved()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        
        var expectedLocation = Location.Create(10, 20);
        var notification = new LocationSearchedNotification(expectedLocation);
         
        _searchHistoryItemRepositoryMock
            .FindByLocationAsync(expectedLocation, cancellationToken)
            .Returns((SearchHistoryItem?)null);
        
        var expectedTime = new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero);
        _timeProviderMock.Now.Returns(expectedTime);

        // Act
        await _sut.Handle(notification, cancellationToken);

        // Assert
        _searchHistoryItemRepositoryMock
            .Received(1)
            .Add(Arg.Is<SearchHistoryItem>(item =>
                item.Location == expectedLocation &&
                item.SearchedAt == expectedTime));
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
    }
    
    [Fact]
    public async Task GivenValidNotification_WhenNotificationIsHandledAndLocationWasSearchedBefore_ThenSearchedLocationIsUpdated()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        
        var expectedLocation = Location.Create(10, 20);
        var notification = new LocationSearchedNotification(expectedLocation);
        
        _timeProviderMock.Now
            .Returns(new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero));
        
        var searchHistoryItem = _searchHistoryItemFactory.Create(expectedLocation);
        _searchHistoryItemRepositoryMock
            .FindByLocationAsync(expectedLocation, cancellationToken)
            .Returns(searchHistoryItem);
        
        var expectedNewTime = new DateTimeOffset(2024, 3, 22, 18, 50, 0, TimeSpan.Zero);
        _timeProviderMock.Now
            .Returns(expectedNewTime);

        // Act
        await _sut.Handle(notification, cancellationToken);

        // Assert
        searchHistoryItem.SearchedAt.Should().Be(expectedNewTime);
        
        _searchHistoryItemRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<SearchHistoryItem>());
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
    }
}