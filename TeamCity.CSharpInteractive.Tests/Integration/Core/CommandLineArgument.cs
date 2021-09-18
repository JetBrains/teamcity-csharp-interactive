namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System;

    internal readonly struct CommandLineArgument
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string Value;

        public CommandLineArgument(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            Value = value;
        }

        public override string ToString() => Value;
    }
}