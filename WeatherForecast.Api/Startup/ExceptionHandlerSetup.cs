using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Domain.Exceptions;

namespace WeatherForecast.Api.Startup;

public static class ExceptionHandlerSetup
{
    public static void UseExceptionHandlers(this WebApplication application)
    {
        if (application.Environment.IsDevelopment())
        {
            application.UseDeveloperExceptionPage();
        }

        application.UseExceptionHandler(builder =>
        {
            builder.Run(HandleExceptionsAsync);
        });
    }

    private static async Task HandleExceptionsAsync(HttpContext context)
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                
        if (exceptionHandler is null)
        {
            return;
        }

        switch (exceptionHandler.Error)
        {
            case WeatherForecastAlreadyExistsException weatherForecastAlreadyExistsException:
                await HandleWeatherForecastAlreadyExistsExceptionAsync(weatherForecastAlreadyExistsException, context);
                break;
            case ValidationException validationException:
                await HandleValidationExceptionAsync(validationException, context);
                break;
        }
    } 
    
    private static async Task HandleWeatherForecastAlreadyExistsExceptionAsync(
        WeatherForecastAlreadyExistsException exception,
        HttpContext context)
    {
        await PrepareResponseAsync(exception.Message, StatusCodes.Status409Conflict, context);
    }
    
    private static async Task HandleValidationExceptionAsync(ValidationException exception, HttpContext context)
    {
        await PrepareResponseAsync(exception.Message, StatusCodes.Status400BadRequest, context);
    }

    private static async Task PrepareResponseAsync(string message, int statusCode, HttpContext context)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = JsonSerializer.Serialize(new ProblemDetails { Detail = message });
        await context.Response.WriteAsync(response);
    }
}