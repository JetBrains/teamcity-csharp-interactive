// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IColorTheme
{
    public ConsoleColor GetConsoleColor(Color color);

    string GetAnsiColor(Color color);
}