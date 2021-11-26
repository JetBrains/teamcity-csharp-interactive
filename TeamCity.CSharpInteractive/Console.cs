namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    // ReSharper disable once ClassNeverInstantiated.Global
    [ExcludeFromCodeCoverage]
    internal class Console : IConsole
    {
        private readonly object _lockObject = new();

        public void Write(params (ConsoleColor? color, string output)[] text)
        {
            lock (_lockObject)
            {
                var foregroundColor = System.Console.ForegroundColor;
                try
                {
                    foreach (var (color, output) in text)
                    {
                        if (color.HasValue)
                        {
                            System.Console.ForegroundColor = color.Value;
                        }

                        System.Console.Out.Write(output);
                    }
                }
                finally
                {
                    System.Console.ForegroundColor = foregroundColor;
                }
            }
        }
    }
}