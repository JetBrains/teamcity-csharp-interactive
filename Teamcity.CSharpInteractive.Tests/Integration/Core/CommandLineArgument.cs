namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;

    internal readonly struct CommandLineArgument
    {
        public readonly string Value;

        public CommandLineArgument(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            Value = value;
        }

        public override string ToString() => Value;
    }
}