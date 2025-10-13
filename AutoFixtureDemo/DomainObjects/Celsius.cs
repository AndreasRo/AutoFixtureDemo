namespace AutoFixtureDemo.DomainObjects
{
    public readonly struct Celsius
    {
        public int Value { get; }

        public const int AbsoluteZero = -273;
        public const int MaxValue = 200;

        public Celsius(int value)
        {
            if (value < AbsoluteZero)
                throw new ArgumentOutOfRangeException(nameof(value), $"Temperature cannot be below {AbsoluteZero} °C");
            if (value > MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), $"Temperature cannot be over {MaxValue} °C, that is extremely hot!");
            Value = value;
        }

        // implicit conversions for convenience
        public static implicit operator int(Celsius c) => c.Value;
        public static implicit operator Celsius(int v) => new(v);
        public static explicit operator Celsius(Fahrenheit f) => new(Convert.ToInt32((f.Value - 32) * 5.0 / 9.0));

        public override string ToString() => $"{Value} °C";

        public override bool Equals(object? obj) => obj is Celsius c && c.Value == Value;
        public static bool operator ==(Celsius left, Celsius right) => left.Equals(right);
        public static bool operator !=(Celsius left, Celsius right) => !left.Equals(right);

        public override int GetHashCode() => Value.GetHashCode();
    }
}
