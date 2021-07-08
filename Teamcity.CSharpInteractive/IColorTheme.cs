// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal interface IColorTheme
    {
        public ConsoleColor GetConsoleColor(Color color);

        string GetAnsiColor(Color color);
    }
}