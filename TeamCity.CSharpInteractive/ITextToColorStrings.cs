namespace TeamCity.CSharpInteractive;

internal interface ITextToColorStrings
{
    IEnumerable<(ConsoleColor? color, string text)> Convert(string text, ConsoleColor? defaultColor);
}