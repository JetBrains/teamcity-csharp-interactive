// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

using System.Text;

internal class MSBuildArgumentsTool : IMSBuildArgumentsTool
{
    private const char Marker = '%';
    
    public string Unescape(string escaped)
    {
        if (string.IsNullOrEmpty(escaped))
        {
            return escaped;
        }

        var markerIndex = escaped.IndexOf(Marker);
        if (markerIndex == -1)
        {
            return escaped;
        }
        
        var unescaped = new StringBuilder(escaped.Length);
        var length = escaped.Length;
        var position = 0;
        while (markerIndex != -1)
        {
            if (
                markerIndex <= length - 3
                && TryDecode(escaped[markerIndex + 1], out var first)
                && TryDecode(escaped[markerIndex + 2], out var second))
            {
                unescaped.Append(escaped, position, markerIndex - position);
                unescaped.Append((char)((first << 4) + second));
                position = markerIndex + 3;
            }

            markerIndex = escaped.IndexOf(Marker, markerIndex + 1);
        }

        unescaped.Append(escaped, position, length - position);
        return unescaped.ToString();
    }
    
    private static bool TryDecode(char ch, out int value)
    {
        switch (ch)
        {
            case >= '0' and <= '9':
                value = ch - '0';
                return true;
            
            case >= 'A' and <= 'F':
                value = ch - 'A' + 10;
                return true;
            
            case >= 'a' and <= 'f':
                value = ch - 'a' + 10;
                return true;
            
            default:
                value = default;
                return false;
        }
    }
}