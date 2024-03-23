using FluentAssertions;
using NSubstitute;
using WeatherForecast.Domain.Factories;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Providers;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class SearchHistoryItemFactoryTests
{
    private readonly ITimeProvider _timeProviderMock;
    
    private readonly SearchHistoryItemFactory _sut;

    public SearchHistoryItemFactoryTests()
    {
        _timeProviderMock = Substitute.For<ITimeProvider>();
        _sut = new SearchHistoryItemFactory(_timeProviderMock);
    }
    
    [Fact]
    public void GivenValidValues_WhenSearchHistoryItemIsCreated_ThenAllPropertiesAreInitialized()
    {
        // Arrange
        var expectedLocation = Location.Create(10, 20);
        var expectedTime = new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero);
        
        _timeProviderMock.Now.Returns(expectedTime);

        // Act
        var searchHistoryItem = _sut.Create(expectedLocation);

        // Assert
        searchHistoryItem.Id.Should().BeEmpty();
        searchHistoryItem.Location.Should().Be(expectedLocation);
        searchHistoryItem.SearchedAt.Should().Be(expectedTime);
    }
}