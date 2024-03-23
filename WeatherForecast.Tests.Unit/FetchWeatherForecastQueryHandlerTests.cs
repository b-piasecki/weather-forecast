using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherForecast.Application.Clients;
using WeatherForecast.Application.Notifications;
using WeatherForecast.Application.Queries;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Repositories;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class FetchWeatherForecastQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IWeatherForecastRepository _weatherForecastRepositoryMock;
    private readonly IWeatherForecastClient _weatherForecastClientMock;
    private readonly IMediator _mediatorMock;
    
    private readonly FetchWeatherForecastQueryHandler _sut;
    
    public FetchWeatherForecastQueryHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        _weatherForecastRepositoryMock = Substitute.For<IWeatherForecastRepository>();
        _unitOfWorkMock.WeatherForecasts.Returns(_weatherForecastRepositoryMock);

        _weatherForecastClientMock = Substitute.For<IWeatherForecastClient>();
        _mediatorMock = Substitute.For<IMediator>();
        
        var loggerMock = Substitute.For<ILogger<FetchWeatherForecastQueryHandler>>();
        _sut = new FetchWeatherForecastQueryHandler(
            _unitOfWorkMock,
            _weatherForecastClientMock,
            _mediatorMock,
            loggerMock);
    }
    
    [Fact]
    public async Task GivenQueryNotForNewestForecast_WhenQueryIsHandledAndThereIsLocalWeatherForecast_ThenLocalWeatherForecastIsReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), false);
        var cancellationToken = default(CancellationToken);

        var expectedWeatherForecast = WeatherForecastData.Create(
            query.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastRepositoryMock
            .FindByLocationAsync(query.Location, cancellationToken)
            .Returns(expectedWeatherForecast);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        weatherForecast.Should().BeEquivalentTo(expectedWeatherForecast);

        await _weatherForecastClientMock
            .DidNotReceive()
            .FetchWeatherForecastAsync(query.Location, cancellationToken);
        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Is<WeatherForecastData>(wf => wf == expectedWeatherForecast));
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
    
    [Fact]
    public async Task GivenQueryNotForNewestForecast_WhenQueryIsHandledAndThereIsNoLocalWeatherForecastAndThereIsClientWeatherForecast_ThenClientWeatherForecastIsSavedAndReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), false);
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(query.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);
        
        var expectedWeatherForecast = WeatherForecastData.Create(
            query.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(query.Location, cancellationToken)
            .Returns(expectedWeatherForecast);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        weatherForecast.Should().BeEquivalentTo(expectedWeatherForecast);
        
        _weatherForecastRepositoryMock
            .Received(1)
            .Add(Arg.Is<WeatherForecastData>(wf =>wf == expectedWeatherForecast));
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
    
    [Fact]
    public async Task GivenQueryNotForNewestForecast_WhenQueryIsHandledAndThereIsNoLocalWeatherForecastAndThereIsNoClientWeatherForecast_ThenNullIsReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), false);
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(query.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(query.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        weatherForecast.Should().BeNull();
        
        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
    
    [Fact]
    public async Task GivenQueryForNewestForecast_WhenQueryIsHandledAndThereIsNoLocalWeatherForecastAndThereIsClientWeatherForecast_ThenClientWeatherForecastIsSavedAndReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), true);
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(query.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);
        
        var expectedWeatherForecast = WeatherForecastData.Create(
            query.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(query.Location, cancellationToken)
            .Returns(expectedWeatherForecast);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        weatherForecast.Should().BeEquivalentTo(expectedWeatherForecast);

        _weatherForecastRepositoryMock
            .Received(1)
            .Add(Arg.Is<WeatherForecastData>(wf => wf == expectedWeatherForecast));
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
    
    [Fact]
    public async Task GivenQueryForNewestForecast_WhenQueryIsHandledAndThereIsLocalWeatherForecastAndThereIsClientWeatherForecast_ThenLocalWeatherForecastIsUpdatedAndReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), true);
        var cancellationToken = default(CancellationToken);
        
        var existingWeatherForecast = WeatherForecastData.Create(
            query.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastRepositoryMock
            .FindByLocationAsync(query.Location, cancellationToken)
            .Returns(existingWeatherForecast);
        
        var newWeatherForecast = WeatherForecastData.Create(
            query.Location,
            new DateTimeOffset(2024, 3, 22, 18, 50, 0, TimeSpan.Zero),
            35,
            45,
            55);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(query.Location, cancellationToken)
            .Returns(newWeatherForecast);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        existingWeatherForecast.Time.Should().Be(newWeatherForecast.Time);
        existingWeatherForecast.Temperature.Should().Be(newWeatherForecast.Temperature);
        existingWeatherForecast.Humidity.Should().Be(newWeatherForecast.Humidity);
        existingWeatherForecast.WindSpeed.Should().Be(newWeatherForecast.WindSpeed);
        
        weatherForecast.Should().BeEquivalentTo(existingWeatherForecast);

        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
    
    [Fact]
    public async Task GivenQueryForNewestForecast_WhenQueryIsHandledAndThereIsNoClientWeatherForecast_ThenNullIsReturned()
    {
        // Arrange
        var query = new FetchWeatherForecastQuery(Location.Create(10, 20), true);
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(query.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);

        // Act
        var weatherForecast = await _sut.Handle(query, cancellationToken);

        // Assert
        weatherForecast.Should().BeNull();

        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
        await _mediatorMock
            .Received(1)
            .Publish(
                Arg.Is<LocationSearchedNotification>(notification => notification.Location == query.Location),
                cancellationToken);
    }
}