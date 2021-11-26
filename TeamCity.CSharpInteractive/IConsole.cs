namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface IConsole
    {
        void Write(params (ConsoleColor? color, string output)[] text);
    }
}