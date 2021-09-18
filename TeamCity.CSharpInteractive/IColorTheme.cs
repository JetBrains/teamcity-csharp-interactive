// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using Contracts;

    internal interface IColorTheme
    {
        public ConsoleColor GetConsoleColor(Color color);

        string GetAnsiColor(Color color);
    }
}