using System;
using System.Text.RegularExpressions;

namespace AutoFixtureDemo.DomainObjects
{
    public readonly struct Text
    {
        private static readonly Regex _alphaNum = new("^[a-zA-Z0-9 ]*$", RegexOptions.Compiled);

        public string Value { get; }

        public const int MaxLength = 1000;

        public Text(string value)
        {
            value = value ?? string.Empty;
            if (value.Length > MaxLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"Text cannot be longer than {MaxLength} characters.");

            if (!_alphaNum.IsMatch(value))
                throw new ArgumentException("Text must be alphanumeric (spaces allowed).", nameof(value));

            Value = value;
        }

        public static implicit operator string(Text t) => t.Value;
        public static implicit operator Text(string s) => new Text(s);

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is Text t && t.Value == Value;
        public override int GetHashCode() => Value.GetHashCode();
    }
}
