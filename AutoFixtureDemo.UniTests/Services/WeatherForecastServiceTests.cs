using Moq;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.UnitTests.AutoFixtureCustomizations;
using Xunit.Abstractions;

namespace AutoFixtureDemo.UnitTests.Services;

public class WeatherForecastServiceTests(ITestOutputHelper output)
{
    [Theory]
    [AnyFixturesInlineData([typeof(EntityCustomization), typeof(ServiceSetupCustomization)])]
    public void GetForecastsForLocation_ReturnsMappedLocation_WhenRepositoryReturnsEntity(LocationEntity locationEntity, Mock<ILocationRepository> mockRepo, WeatherForecastService service)
    {
        // Arrange
        locationEntity.Forecasts.ForEach(f => f.Location = locationEntity);
        mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns(locationEntity);

        // Act
        var result = service.GetForecastsForLocation(locationEntity.City);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(locationEntity.City, result.City.Value);
        Assert.Equal(locationEntity.Country, result.Country.Value);
        Assert.Equal(locationEntity.Forecasts.Count, result.Forecasts.Count);
        Assert.Contains(result.Forecasts, f => locationEntity.Forecasts.Any(e => e.Date == f.Date && e.TemperatureC == f.TemperatureC.Value && e.Summary == f.Summary));
    }

    [Theory]
    [AnyFixturesInlineData([typeof(ServiceSetupCustomization)])]
    public void GetForecastsForLocation_ReturnsNull_WhenRepositoryReturnsNull(Mock<ILocationRepository> mockRepo, WeatherForecastService service)
    {
        // Arrange
        mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns<LocationEntity?>(null!);

        // Act
        var result = service.GetForecastsForLocation("UnknownCity");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [AnyFixturesAutoData([typeof(DomainObjectsCustomization)])]
    public void Pitfalls(Location locationEntity)
    {
        output.WriteLine($"Location: {locationEntity.City}, {locationEntity.Country}, {locationEntity.Forecasts.Count} forecasts");
        Assert.Equal("MyCity", locationEntity.City.Value);
        Assert.Equal("MyCountry", locationEntity.Country.Value);
        Assert.All(locationEntity.Forecasts, f => Assert.Equal(WeatherSummary.Mild, f.Summary));
    }
}
