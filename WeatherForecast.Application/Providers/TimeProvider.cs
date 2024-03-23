using WeatherForecast.Domain.Providers;

namespace WeatherForecast.Application.Providers;

public sealed class TimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}