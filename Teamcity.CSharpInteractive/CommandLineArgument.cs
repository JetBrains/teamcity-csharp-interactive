namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Text;

    internal readonly struct CommandLineArgument
    {
        public readonly CommandLineArgumentType ArgumentType;
        public readonly string Value;
        public readonly string Key;

        public CommandLineArgument(CommandLineArgumentType argumentType, string value = "", string key = "")
        {
            ArgumentType = argumentType;
            Value = value;
            Key = key;
        }

        public override bool Equals(object? obj) => 
            obj is CommandLineArgument other && (ArgumentType == other.ArgumentType && Value == other.Value && Key == other.Key);

        public override int GetHashCode() => HashCode.Combine((int) ArgumentType, Value, Key);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(ArgumentType);
            if (!string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Value))
            {
                sb.Append($": {Key}={Value}");
            }
            else
            {
                sb.Append($": {Value}");
            }
            
            return sb.ToString();
        }
    }
}