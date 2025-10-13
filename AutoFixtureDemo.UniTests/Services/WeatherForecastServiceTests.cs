using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.Mapping;
using AutoFixtureDemo.Services;
using AutoMapper;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoFixtureDemo.UnitTests.Services;

public class WeatherForecastServiceTests
{
    private readonly Mock<ILocationRepository> _mockRepo = new();
    private readonly WeatherForecastService _service;

    public WeatherForecastServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new LoggerFactory());
        var mapper = mapperConfig.CreateMapper();

        _service = new WeatherForecastService(_mockRepo.Object, mapper);
    }
    
    [Fact]
    public void GetForecastsForLocation_ReturnsMappedLocation_WhenRepositoryReturnsEntity()
    {
        // Arrange
        var fixture = new Fixture();
        
        fixture.Register(() => DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(0, 365))));

        var forecasts = fixture.Build<WeatherForecastEntity>()
            .Without(f => f.Location)
            .CreateMany(5)
            .ToList();

        var locationEntity = fixture.Build<LocationEntity>()
            .With(l => l.City, "TestCity")
            .With(l => l.Country, "TestCountry")
            .With(l => l.Forecasts, forecasts)
            .Create();

        forecasts.ForEach(f => f.Location = locationEntity);

        _mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns(locationEntity);

        // Act
        var result = _service.GetForecastsForLocation(locationEntity.City);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestCity", result.City.Value);
        Assert.Equal("TestCountry", result.Country.Value);
        Assert.Equal(locationEntity.Forecasts.Count, result.Forecasts.Count);
        Assert.Contains(result.Forecasts, f => locationEntity.Forecasts.Any(e => e.Date == f.Date && e.TemperatureC == f.TemperatureC.Value && e.Summary == f.Summary));
    }

    [Fact]
    public void GetForecastsForLocation_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns<LocationEntity?>(null!);

        // Act
        var result = _service.GetForecastsForLocation("UnknownCity");

        // Assert
        Assert.Null(result);
    }
}
