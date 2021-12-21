namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    [ExcludeFromCodeCoverage]
    internal readonly record struct CommandLineArgument(CommandLineArgumentType ArgumentType, string Value = "", string Key= "")
    {
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