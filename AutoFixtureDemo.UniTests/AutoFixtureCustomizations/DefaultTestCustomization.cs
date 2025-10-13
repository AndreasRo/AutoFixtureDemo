using AutoFixture;
using AutoFixtureDemo.Database.Entities;
using AutoFixtureDemo.DomainObjects;

namespace AutoFixtureDemo.UnitTests.AutoFixtureCustomizations
{
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
}
