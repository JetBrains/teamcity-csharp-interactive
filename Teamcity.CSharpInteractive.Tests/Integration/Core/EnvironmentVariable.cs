namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;

    internal readonly struct EnvironmentVariable
    {
        public readonly string Name;
        public readonly string Value;

        public EnvironmentVariable(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(nameof(name));
            Name = name;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString() => $"{Name}={Value}";
    }
}