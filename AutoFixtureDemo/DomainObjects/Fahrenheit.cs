namespace AutoFixtureDemo.DomainObjects
{
    public readonly struct Fahrenheit
    {
        public int Value { get; }

        // Absolute zero in Fahrenheit is -459.67, but we use integer representation and floor to -459
        public const int AbsoluteZero = -459;

        public Fahrenheit(int value)
        {
            if (value < AbsoluteZero)
                throw new ArgumentOutOfRangeException(nameof(value), $"Temperature cannot be below {AbsoluteZero} °F");
            Value = value;
        }

        // implicit conversions for convenience
        public static implicit operator int(Fahrenheit f) => f.Value;
        public static implicit operator Fahrenheit(int v) => new(v);
        public static explicit operator Fahrenheit(Celsius c) => new(Convert.ToInt32(c.Value * 9.0 / 5.0 + 32));

        public override string ToString() => $"{Value} °F";

        public override bool Equals(object? obj) => obj is Fahrenheit f && f.Value == Value;
        public static bool operator ==(Fahrenheit left, Fahrenheit right) => left.Equals(right);
        public static bool operator !=(Fahrenheit left, Fahrenheit right) => !left.Equals(right);        

        public override int GetHashCode() => Value.GetHashCode();
    }
}
