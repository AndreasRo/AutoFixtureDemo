using System.Text.RegularExpressions;

namespace AutoFixtureDemo.DomainObjects;

public readonly partial struct Text : IEquatable<Text>
{
    [GeneratedRegex("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled)]
    private static partial Regex TextRegex();
    private static readonly Regex AlphaNum = TextRegex();

    public string Value { get; }

    public const int MaxLength = 1000;

    public Text(string value)
    {
        if (value.Length > MaxLength)
            throw new ArgumentOutOfRangeException(nameof(value), $"Text cannot be longer than {MaxLength} characters.");

        if (!AlphaNum.IsMatch(value))
            throw new ArgumentException("Text must be alphanumeric (spaces allowed).", nameof(value));

        Value = value;
    }

    public static implicit operator string(Text t) => t.Value;
    public static implicit operator Text(string s) => new(s);

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is Text t && t.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Text left, Text right) => left.Equals(right);
    public static bool operator !=(Text left, Text right) => !left.Equals(right);

    public bool Equals(Text other) => Value == other.Value;
}
