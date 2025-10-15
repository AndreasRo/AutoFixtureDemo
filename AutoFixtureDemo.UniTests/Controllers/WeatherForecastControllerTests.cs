using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Controllers.DTO;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Services;
using AutoFixtureDemo.UnitTests.AutoFixtureCustomizations;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AutoFixtureDemo.UnitTests.Controllers;

public class WeatherForecastControllerTests
{
    [Theory]
    [DomainPrimitivesAutoData]
    public void Get_ReturnsForecastsFromService(Location location, Mock<IWeatherForecastService> mockService, WeatherForecastController controller)
    {
        // Arrange
        mockService.Setup(s => s.GetForecastsForLocation(location.City.Value)).Returns(location);

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