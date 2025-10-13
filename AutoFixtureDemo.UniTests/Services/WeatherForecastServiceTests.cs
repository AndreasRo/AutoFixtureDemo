using Moq;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.Mapping;
using AutoMapper;
using AutoFixture;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.UnitTests.AutoFixtureCustomizations;
using Microsoft.Extensions.Logging;

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
        fixture.Customize(new DefaultTestCustomization());

        var forecasts = fixture.Build<WeatherForecastEntity>()
            .Without(f => f.Location)
            .With(f => f.TemperatureC, () => fixture.Create<Celsius>().Value)
            .CreateMany(5)
            .ToList();

        var locationEntity = fixture.Build<LocationEntity>()
            .With(l => l.Forecasts, forecasts)
            .With(f => f.City, () => fixture.Create<Text>().Value)
            .With(f => f.Country, () => fixture.Create<Text>().Value)
            .Create();

        forecasts.ForEach(f => f.Location = locationEntity);

        _mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns(locationEntity);

        // Act
        var result = _service.GetForecastsForLocation(locationEntity.City);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(locationEntity.City, result.City.Value);
        Assert.Equal(locationEntity.Country, result.Country.Value);
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
