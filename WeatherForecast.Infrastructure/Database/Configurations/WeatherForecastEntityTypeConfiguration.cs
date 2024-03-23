﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Database.Configurations;

public sealed class WeatherForecastEntityTypeConfiguration : IEntityTypeConfiguration<WeatherForecastData>
{
    public void Configure(EntityTypeBuilder<WeatherForecastData> builder)
    {
        builder.ToTable("WeatherForecasts");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .IsRequired();
        
        builder.OwnsOne(e => e.Location, ConfigureOwnedLocations);
        
        builder
            .Property(e => e.Time)
            .IsRequired();
        
        builder
            .Property(e => e.Temperature)
            .HasPrecision(5, 2)
            .IsRequired();
        
        builder
            .Property(e => e.Humidity)
            .HasPrecision(5, 2)
            .IsRequired();
        
        builder
            .Property(e => e.WindSpeed)
            .HasPrecision(5, 2)
            .IsRequired();
    }
    
    private void ConfigureOwnedLocations(OwnedNavigationBuilder<WeatherForecastData, Location> builder)
    {
        builder
            .Property(e => e.Latitude)
            .HasColumnName("Latitude")
            .HasPrecision(10, 7)
            .IsRequired();

        builder
            .Property(e => e.Longitude)
            .HasColumnName("Longitude")
            .HasPrecision(10, 7)
            .IsRequired();

        builder
            .HasIndex(e => new { e.Longitude, e.Latitude })
            .IsUnique();
    }
}