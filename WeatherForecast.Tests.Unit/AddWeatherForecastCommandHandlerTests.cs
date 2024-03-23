using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherForecast.Application.Clients;
using WeatherForecast.Application.Commands;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Repositories;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class AddWeatherForecastCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IWeatherForecastRepository _weatherForecastRepositoryMock;
    private readonly IWeatherForecastClient _weatherForecastClientMock;
    
    private readonly AddWeatherForecastCommandHandler _sut;
    
    public AddWeatherForecastCommandHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        _weatherForecastRepositoryMock = Substitute.For<IWeatherForecastRepository>();
        _unitOfWorkMock.WeatherForecasts.Returns(_weatherForecastRepositoryMock);
        
        _weatherForecastClientMock = Substitute.For<IWeatherForecastClient>();
        
        var loggerMock = Substitute.For<ILogger<AddWeatherForecastCommandHandler>>();
        _sut = new AddWeatherForecastCommandHandler(_unitOfWorkMock, _weatherForecastClientMock, loggerMock);
    }
    
    [Fact]
    public async Task GivenValidCommand_WhenCommandIsHandledAndWeatherForecastDoesNotExistAndThereIsClientWeatherForecast_ThenClientWeatherForecastIsSavedAndTrueIsReturned()
    {
        // Arrange
        var command = new AddWeatherForecastCommand(Location.Create(10, 20));
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(command.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);
        
        var weatherForecast = WeatherForecastData.Create(
            command.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(command.Location, cancellationToken)
            .Returns(weatherForecast);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        _weatherForecastRepositoryMock
            .Received(1)
            .Add(weatherForecast);
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
    }
    
    [Fact]
    public async Task GivenValidCommand_WhenCommandIsHandledAndWeatherForecastAlreadyExists_ThenWeatherForecastAlreadyExistsExceptionIsThrown()
    {
        // Arrange
        var command = new AddWeatherForecastCommand(Location.Create(10, 20));
        var cancellationToken = default(CancellationToken);
        
        var weatherForecast = WeatherForecastData.Create(
            command.Location,
            new DateTimeOffset(2024, 3, 22, 18, 0, 0, TimeSpan.Zero),
            30,
            40,
            50);
        _weatherForecastRepositoryMock
            .FindByLocationAsync(command.Location, cancellationToken)
            .Returns(weatherForecast);

        // Act
        var action = () => _sut.Handle(command, cancellationToken);

        // Assert
        await action
            .Should()
            .ThrowAsync<WeatherForecastAlreadyExistsException>()
            .WithMessage($"Weather forecast already exists for location: ({command.Location.Latitude}, {command.Location.Longitude})");
        
        await _weatherForecastClientMock
            .DidNotReceive()
            .FetchWeatherForecastAsync(command.Location, cancellationToken);
        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
    }
    
    [Fact]
    public async Task GivenValidCommand_WhenCommandIsHandledAndWeatherForecastDoesNotExistAndThereIsNoClientWeatherForecast_ThenFalseIsReturned()
    {
        // Arrange
        var command = new AddWeatherForecastCommand(Location.Create(10, 20));
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(command.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);
        _weatherForecastClientMock
            .FetchWeatherForecastAsync(command.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeFalse();
        
        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Add(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
    }
}