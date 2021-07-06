namespace Teamcity.CSharpInteractive
{
    using System;

    internal readonly struct CommandLineArgument
    {
        public readonly CommandLineArgumentType ArgumentType;
        public readonly string Value;

        public CommandLineArgument(CommandLineArgumentType argumentType, string value = "")
        {
            ArgumentType = argumentType;
            Value = value;
        }

        public override bool Equals(object? obj) =>
            obj is CommandLineArgument other && (ArgumentType == other.ArgumentType && Value == other.Value);

        public override int GetHashCode() => HashCode.Combine((int) ArgumentType, Value);

        public override string ToString() => $"{ArgumentType}: \"{Value}\"";
    }
}