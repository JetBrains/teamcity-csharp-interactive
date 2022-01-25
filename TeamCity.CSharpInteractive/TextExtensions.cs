namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using HostApi;

[ExcludeFromCodeCoverage]
internal static class TextExtensions
{
    public static Text[] WithDefaultColor(this Text[] text, Color defaultColor)
    {
        var newText = new Text[text.Length];
        for (var i = 0; i < newText.Length; i++)
        {
            var (value, color) = text[i];
            newText[i] = new Text(value, color == Color.Default ? defaultColor : color);
        }

        return newText;
    }

    public static string ToSimpleString(this IEnumerable<Text> text) =>
        string.Join("", text.Select(i => i.Value));
}