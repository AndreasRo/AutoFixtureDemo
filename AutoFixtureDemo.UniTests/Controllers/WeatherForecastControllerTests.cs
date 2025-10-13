using AutoFixture;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Controllers.DTO;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoFixtureDemo.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _mockService = new();

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void Get_ReturnsForecastsFromService()
    {
        // Arrange
        var fixture = new Fixture();

        var forecasts = fixture.Build<WeatherForecast>()
            .CreateMany(5)
            .ToList();

        var location = fixture.Build<Location>()
            .With(l => l.Forecasts, forecasts)
            .Create();

        _mockService.Setup(s => s.GetForecastsForLocation(location.City.Value)).Returns(location);

        // create mapper
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<Mapping.MappingProfile>(), new LoggerFactory());
        var mapper = mapperConfig.CreateMapper();

        var controller = new WeatherForecastController(_mockService.Object, mapper);

        // Act
        var result = controller.Get(location.City.Value);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<AutoFixtureDemo.Controllers.DTO.LocationDto>(ok.Value);

        Assert.Equal(location.City.Value, dto.City);
        Assert.Equal(location.Country.Value, dto.Country);
        Assert.Equal(location.Forecasts.Count, dto.Forecasts.Count);
    }
}