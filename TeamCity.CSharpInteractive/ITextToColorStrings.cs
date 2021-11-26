namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;

    internal interface ITextToColorStrings
    {
        IEnumerable<(ConsoleColor? color, string text)> Convert(string text, ConsoleColor? defaultColor);
    }
}