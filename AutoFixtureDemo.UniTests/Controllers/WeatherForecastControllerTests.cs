using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Controllers.DTO;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.UnitTests.AutoFixtureCustomizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoFixtureDemo.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<IWeatherForecastService> _mockService;
    private readonly WeatherForecastController _controller;

    public WeatherForecastControllerTests()
    {
        _mockService = new Mock<IWeatherForecastService>();
        var mapperConfig = new AutoMapper.MapperConfiguration(cfg => cfg.AddProfile<Mapping.MappingProfile>(), new LoggerFactory());
        var mapper = mapperConfig.CreateMapper();

        _controller = new WeatherForecastController(_mockService.Object, mapper);
    }

    [Theory]
    [DomainPrimitivesAutoData]
    public void Get_ReturnsForecastsFromService(Location location)
    {
        // Arrange
        _mockService.Setup(s => s.GetForecastsForLocation(location.City.Value)).Returns(location);

        // Act
        var result = _controller.Get(location.City.Value);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<AutoFixtureDemo.Controllers.DTO.LocationDto>(ok.Value);

        Assert.Equal(location.City.Value, dto.City);
        Assert.Equal(location.Country.Value, dto.Country);
        Assert.Equal(location.Forecasts.Count, dto.Forecasts.Count);
    }
}