using System.Configuration;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Api.Profiles;
using WeatherForecast.Application.Clients;
using WeatherForecast.Application.Queries;
using WeatherForecast.Domain;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Factories;
using WeatherForecast.Domain.Providers;
using WeatherForecast.Domain.Repositories;
using WeatherForecast.Infrastructure;
using WeatherForecast.Infrastructure.Clients.OpenMeteo;
using WeatherForecast.Infrastructure.Database;
using WeatherForecast.Infrastructure.Database.Repositories;

namespace WeatherForecast.Api.Startup;

public static class ServicesStartup
{
    private const string DatabaseConnectionStringName = "WeatherForecastDatabase";
    
    public static void AddWeatherForecastServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<WeatherForecastContext>("SQL Database");
        
        services.AddAutoMapper(typeof(ApiToApplicationProfile).Assembly);
        
        services.AddHttpClient<IWeatherForecastClient, OpenMeteoWeatherForecastClient>(client =>
        {
            var baseUrl = configuration["OpenMeteo:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ConfigurationErrorsException($"OpenMeteo base url is required");
            }
            
            client.BaseAddress = new Uri(baseUrl);
        });
        
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<FetchWeatherForecastQuery>();
        });
        
        services.AddScoped<ITimeProvider, WeatherForecast.Application.Providers.TimeProvider>();
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();
        services.AddScoped<ISearchHistoryItemRepository, SearchHistoryItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<SearchHistoryItemFactory>();
        
        AddDatabase(services, configuration);
    }
    
    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DatabaseConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ConfigurationErrorsException(
                $"'{DatabaseConnectionStringName}' connection string is required");
        }

        services.AddDbContext<WeatherForecastContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        AddScopedDbSet<WeatherForecastData>(services);
        AddScopedDbSet<SearchHistoryItem>(services);
    }

    private static void AddScopedDbSet<TEntity>(IServiceCollection services) where TEntity : class
    {
        services.AddScoped<DbSet<TEntity>>(provider =>
        {
            var context = provider.GetRequiredService<WeatherForecastContext>();
            return context.Set<TEntity>();
        });
    }
}