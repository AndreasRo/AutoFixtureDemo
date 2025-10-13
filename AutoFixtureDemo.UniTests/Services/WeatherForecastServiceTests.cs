using Xunit;
using Moq;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoMapper;
using AutoFixtureDemo.Mapping;
using AutoFixtureDemo.DomainObjects;

public class WeatherForecastServiceTests
{
    private readonly Mock<ILocationRepository> mockRepo = new();
    private readonly WeatherForecastService service;

    public WeatherForecastServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        var mapper = mapperConfig.CreateMapper();

        service = new WeatherForecastService(mockRepo.Object, mapper);
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

        mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns(entity);

        // Act
        var result = service.GetForecastsForLocation("TestCity");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("TestCity", result!.City.Value);
        Assert.Equal("TestCountry", result.Country.Value);
        Assert.Equal(entity.Forecasts.Count, result.Forecasts.Count);
        Assert.Contains(result.Forecasts, f => entity.Forecasts.Any(e => e.Date == f.Date && e.TemperatureC == f.TemperatureC.Value && e.Summary == f.Summary));
    }

    [Fact]
    public void GetForecastsForLocation_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        mockRepo.Setup(r => r.GetLocationByName(It.IsAny<string>())).Returns<LocationEntity?>(null!);

        // Act
        var result = service.GetForecastsForLocation("UnknownCity");

        // Assert
        Assert.Null(result);
    }
}
