using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Mapping;
using AutoFixtureDemo.Services;
using AutoMapper;
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
        var summaries = Enum.GetValues<WeatherSummary>();
        var entity = new LocationEntity
        {
            City = "TestCity",
            Country = "TestCountry",
            Forecasts = [.. Enumerable.Range(1, 5).Select(index => new WeatherForecastEntity
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })]
        };

        _mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns(entity);

        // Act
        var result = _service.GetForecastsForLocation("TestCity");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestCity", result.City.Value);
        Assert.Equal("TestCountry", result.Country.Value);
        Assert.Equal(entity.Forecasts.Count, result.Forecasts.Count);
        Assert.Contains(result.Forecasts, f => entity.Forecasts.Any(e => e.Date == f.Date && e.TemperatureC == f.TemperatureC.Value && e.Summary == f.Summary));
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
