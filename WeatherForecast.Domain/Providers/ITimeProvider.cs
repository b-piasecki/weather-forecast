namespace WeatherForecast.Domain.Providers;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}