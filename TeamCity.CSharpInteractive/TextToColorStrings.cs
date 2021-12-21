// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;

    internal class TextToColorStrings : ITextToColorStrings
    {
        private const string ColorPrefix = "\x001B[";
        private static readonly Dictionary<int, ConsoleColor?> Colors = new()
        {
            {39, default},
            {30, ConsoleColor.Black},
            {31, ConsoleColor.DarkRed},
            {32, ConsoleColor.DarkGreen},
            {33, ConsoleColor.DarkYellow},
            {34, ConsoleColor.DarkBlue},
            {35, ConsoleColor.DarkMagenta},
            {36, ConsoleColor.DarkCyan},
            {37, ConsoleColor.Gray},
            {90, ConsoleColor.DarkGray},
            {91, ConsoleColor.Red},
            {92, ConsoleColor.Green},
            {93, ConsoleColor.Yellow},
            {94, ConsoleColor.Blue},
            {95, ConsoleColor.Magenta},
            {96, ConsoleColor.Cyan},
            {97, ConsoleColor.White}
        };
        
        public IEnumerable<(ConsoleColor? color, string text)> Convert(string text, ConsoleColor? defaultColor)
        {
            if (!text.Contains('\x001B'))
            {
                yield return (defaultColor, text);
                yield break;
            }
            
            foreach (var item in text.Split(ColorPrefix, StringSplitOptions.RemoveEmptyEntries))
            {
                var str = item;
                var ansiColorFinish = str.IndexOf('m');
                var color = defaultColor;
                if (ansiColorFinish >= 0)
                {
                    foreach (var colorStr in str[..ansiColorFinish].Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(colorStr, out var colorId))
                        {
                            // ReSharper disable once InvertIf
                            if (Colors.TryGetValue(colorId, out var curColor))
                            {
                                color = curColor;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    str = str.Substring(ansiColorFinish + 1, str.Length - ansiColorFinish - 1);
                }
                
                yield return (color, str);
            }
        }
    }
}