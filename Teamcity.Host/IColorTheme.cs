// ReSharper disable UnusedMember.Global
namespace Teamcity.Host
{
    using System;

    internal interface IColorTheme
    {
        public ConsoleColor GetConsoleColor(Color color);

        string GetAnsiColor(Color color);
    }
}