using AutoFixture;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;

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

public sealed class EntityAutoDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture()
        .Customize(new EntityCustomization());
    return fixture;
})
{ }

public sealed class DomainPrimitivesAutoDataAttribute() : AutoDataAttribute(() =>
{
            var fixture = new Fixture()
                .Customize(new DefaultTestCustomization());
            return fixture;
        }){}

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