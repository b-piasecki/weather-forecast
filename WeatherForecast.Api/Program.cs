using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Api.Startup;
using WeatherForecast.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddWeatherForecastServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlers();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks(
    "/healthz",
    new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

await MigrateDatabaseAsync(app);

app.Run();
return;

async Task MigrateDatabaseAsync(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    
    try
    {
        application.Logger.LogInformation("Migrating or initializing the database");
        
        var context = scope.ServiceProvider.GetRequiredService<WeatherForecastContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        application.Logger.LogCritical(ex, "An error occurred while migrating or initializing the database");
        throw;
    }
}

public partial class Program
{
    protected Program() { }
}
