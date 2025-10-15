using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Controllers;
using AutoFixtureDemo.Database;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;
using AutoFixtureDemo.Mapping;
using AutoFixtureDemo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AutoFixtureDemo.UnitTests.AutoFixtureCustomizations;

public class DefaultTestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Replace throwing recursion behavior with omission to avoid circular graph exceptions
        foreach (var b in fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToArray())
            fixture.Behaviors.Remove(b);
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Customizations.Add(new BogusBehavior());
        fixture.Customizations.Insert(0, new EnumBuilder());

        fixture.Register(() => new Celsius(Random.Shared.Next(Celsius.AbsoluteZero, Celsius.MaxValue + 1)));

        // Ensure WeatherForecastEntity doesn't try to populate the Location back-reference
        fixture.Customize<WeatherForecastEntity>(c => c.Without(w => w.Location));

        //fixture.Customize<WeatherSummary>(c => c.FromFactory(() => WeatherSummary.Mild));
        // fixture.Register(() => WeatherSummary.Mild);
    }
}

internal class BogusBehavior : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type)
        {
            if (type == typeof(Text))
            {
                return new Text($"Text {Guid.NewGuid():N}");
            }
            if (type == typeof(DateOnly))
            {
                return DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(0, 365)));
            }
            if (type == typeof(WeatherSummary))
            {
                return WeatherSummary.Mild;
            }
        }
        return new NoSpecimen();
    }
}

internal class EnumBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type)
        {
            if (type == typeof(WeatherSummary))
            {
                return WeatherSummary.Mild;
            }
        }
        return new NoSpecimen();
    }
}

    public class EntityCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new DefaultTestCustomization());

            fixture.Customize<WeatherForecastEntity>(c => c
                .Without(w => w.Location)
                .With(f => f.TemperatureC, () => fixture.Create<Celsius>().Value)
            );

            fixture.Customize<LocationEntity>(c => c
                    .With(l => l.Forecasts, () => [.. fixture.CreateMany<WeatherForecastEntity>(5)])
                    .With(f => f.City, () => fixture.Create<Text>().Value)
                    .With(f => f.Country, () => fixture.Create<Text>().Value)
            );

        }
    }

public class DomainObjectsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize(new DefaultTestCustomization());

        fixture.Customize<Location>(c => c.FromFactory(() => new("MyCity", "MyCountry", [.. fixture.CreateMany<WeatherForecast>(5)]))
        .Without(x => x.City)
        .Without(x => x.Country));
        // fixture.Register(() => new Location("MyCity", "MyCountry", [.. fixture.CreateMany<WeatherForecast>(5)]));
        //fixture.Customize<WeatherSummary>(c => c.FromFactory(() => WeatherSummary.Mild));
    }
}

public class ServiceSetupCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize(new AutoMoqCustomization());
        fixture.Register(() =>
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), new LoggerFactory());
            return mapperConfig.CreateMapper();
        });
        fixture.Freeze<Mock<ILocationRepository>>();
        fixture.Freeze<Mock<IWeatherForecastService>>();
        fixture.Customize<WeatherForecastController>(c => c
                .FromFactory(() =>
                {
                    var forecastServiceMock = fixture.Create<Mock<IWeatherForecastService>>();
                    var mapper = fixture.Create<IMapper>();
                    return new WeatherForecastController(forecastServiceMock.Object, mapper);
                })
                .With(x => x.ControllerContext, new ControllerContext())
        );
    }
}

public sealed class EntityAutoDataAttribute() : AutoDataAttribute(() => new Fixture()
    .Customize(new EntityCustomization()))
{ }

public sealed class DomainPrimitivesAutoDataAttribute() : AutoDataAttribute(() => new Fixture()
    .Customize(new DefaultTestCustomization())
    .Customize(new ServiceSetupCustomization())){}

public sealed class AnyFixturesAutoDataAttribute(Type[] customizationTypes) : AutoDataAttribute(() =>
         {
             var fixture = new Fixture();
             foreach (var customizationType in customizationTypes)
             {
                 if (Activator.CreateInstance(customizationType) is ICustomization customization)
                 {
                     fixture.Customize(customization);
                 }
                 else
                 {
                     throw new ArgumentException($"Type {customizationType.FullName} does not implement ICustomization");
                 }
             }
             return fixture;
         })
{
}

public sealed class EntityInlineDataAttribute(params object[] values)
    : InlineAutoDataAttribute(new EntityAutoDataAttribute(), values);

public sealed class AnyFixturesInlineDataAttribute(Type[] customizationTypes, params object[] values)
    : InlineAutoDataAttribute(new AnyFixturesAutoDataAttribute(customizationTypes), values);