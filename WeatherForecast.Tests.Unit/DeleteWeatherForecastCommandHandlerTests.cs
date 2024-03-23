using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WeatherForecast.Application.Commands;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;
using WeatherForecast.Domain.Repositories;
using Xunit;

namespace WeatherForecast.Tests.Unit;

public sealed class DeleteWeatherForecastCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IWeatherForecastRepository _weatherForecastRepositoryMock;
    
    private readonly DeleteWeatherForecastCommandHandler _sut;
    
    public DeleteWeatherForecastCommandHandlerTests()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        
        _weatherForecastRepositoryMock = Substitute.For<IWeatherForecastRepository>();
        _unitOfWorkMock.WeatherForecasts.Returns(_weatherForecastRepositoryMock);
        
        var loggerMock = Substitute.For<ILogger<DeleteWeatherForecastCommandHandler>>();
        _sut = new DeleteWeatherForecastCommandHandler(_unitOfWorkMock, loggerMock);
    }
    
    [Fact]
    public async Task GivenValidCommand_WhenCommandIsHandledAndThereIsWeatherForecast_ThenWeatherForecastIsRemovedAndTrueIsReturned()
    {
        // Arrange
        var command = new DeleteWeatherForecastCommand(Location.Create(10, 20));
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
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        _weatherForecastRepositoryMock
            .Received(1)
            .Remove(weatherForecast);
        await _unitOfWorkMock
            .Received(1)
            .SaveChangesAsync(cancellationToken);
    }
    
    [Fact]
    public async Task GivenValidCommand_WhenCommandIsHandledAndThereIsNoWeatherForecast_ThenFalseIsReturned()
    {
        // Arrange
        var command = new DeleteWeatherForecastCommand(Location.Create(10, 20));
        var cancellationToken = default(CancellationToken);
        
        _weatherForecastRepositoryMock
            .FindByLocationAsync(command.Location, cancellationToken)
            .Returns((WeatherForecastData?)null);

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.Should().BeFalse();
        
        _weatherForecastRepositoryMock
            .DidNotReceive()
            .Remove(Arg.Any<WeatherForecastData>());
        await _unitOfWorkMock
            .DidNotReceive()
            .SaveChangesAsync(cancellationToken);
    }
}