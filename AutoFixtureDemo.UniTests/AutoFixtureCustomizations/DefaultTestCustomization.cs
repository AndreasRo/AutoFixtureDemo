using AutoFixture;
using AutoFixture.Xunit2;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.UnitTests.AutoFixtureCustomizations

public class DefaultTestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        // Replace throwing recursion behavior with omission to avoid circular graph exceptions
        foreach (var b in fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToArray())
            fixture.Behaviors.Remove(b);
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Register a valid DateOnly generator (within next year)
        fixture.Register(() => DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(0, 365))));

        // Register alphanumeric string generator for City/Country so Text validation passes
        fixture.Register(() => new Text($"S{Guid.NewGuid():N}"));
        fixture.Register(() => new Celsius(Random.Shared.Next(Celsius.AbsoluteZero, Celsius.MaxValue + 1)));

        // Ensure WeatherForecastEntity doesn't try to populate the Location back-reference
        fixture.Customize<WeatherForecastEntity>(c => c.Without(w => w.Location));
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

public sealed class EntityAutoDataAttribute : AutoDataAttribute
{
    public EntityAutoDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new EntityCustomization());
            return fixture;
        })
    { }
}

public sealed class DomainPrimitivesAutoDataAttribute : AutoDataAttribute
{
    public DomainPrimitivesAutoDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new DefaultTestCustomization());
            return fixture;
        })
    { }
}

public sealed class EntityInlineDataAttribute(params object[] values)
    : InlineAutoDataAttribute(new EntityAutoDataAttribute(), values);