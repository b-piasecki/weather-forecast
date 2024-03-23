using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Database.Configurations;

public sealed class SearchHistoryItemEntityTypeConfiguration : IEntityTypeConfiguration<SearchHistoryItem>
{
    public void Configure(EntityTypeBuilder<SearchHistoryItem> builder)
    {
        builder.ToTable("SearchHistory");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .IsRequired();
        
        builder.OwnsOne(e => e.Location, ConfigureOwnedLocations);
        
        builder
            .Property(e => e.SearchedAt)
            .IsRequired();
    }
    
    private void ConfigureOwnedLocations(OwnedNavigationBuilder<SearchHistoryItem, Location> builder)
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