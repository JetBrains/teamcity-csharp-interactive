namespace TeamCity.CSharpInteractive;

using System.Text;
using HostApi;

internal static class StartInfoExtensions
{
    public static string GetDescription(this IStartInfo? startInfo, int? processId = default)
    {
        var sb = new StringBuilder();
        if (processId.HasValue)
        {
            sb.Append(processId.Value);
        }

        var shortName = startInfo?.ShortName;
        // ReSharper disable once InvertIf
        if (!string.IsNullOrWhiteSpace(shortName))
        {
            if (sb.Length != 0)
            {
                sb.Append(' ');
            }

            sb.Append(shortName.EscapeArg());
        }

        if (sb.Length == 0)
        {
            sb.Append("The");
        }

        return sb.ToString();
    }
    
    public static string EscapeArg(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text ?? string.Empty;
        }

        // ReSharper disable once InvertIf
        if (text.Contains(' '))
        {
            var trimmed = text.Trim();
            if (!trimmed.StartsWith('"') && !trimmed.EndsWith('"'))
            {
                return $"\"{text}\"";
            }
        }

        return text;
    }
}