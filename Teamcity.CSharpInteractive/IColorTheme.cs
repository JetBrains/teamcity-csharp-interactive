namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IColorTheme
    {
        public ConsoleColor GetConsoleColor(Color color);

        string GetAnsiColor(Color color);
    }
}