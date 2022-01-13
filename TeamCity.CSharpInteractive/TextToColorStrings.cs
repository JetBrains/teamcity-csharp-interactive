// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class TextToColorStrings : ITextToColorStrings
    {
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
            var curColor = defaultColor;
            foreach (var (color, str) in Split(text))
            {
                if (!string.IsNullOrWhiteSpace(color))
                {
                    foreach (var colorStr in color.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!int.TryParse(colorStr, out var colorId) || !Colors.TryGetValue(colorId, out var colorVal) || !colorVal.HasValue)
                        {
                            continue;
                        }

                        curColor = colorVal.Value;
                        break;
                    }
                }

                yield return (curColor, str);
            }
        }

        internal static IEnumerable<(string color, string text)> Split(string text)
        {
            var sb = new StringBuilder(text.Length);
            var isColor = false;
            var color = string.Empty;
            foreach (var ch in text)
            {
                switch (ch)
                {
                    case '\x001B':
                        if (!isColor)
                        {
                            isColor = true;
                        }
                        else
                        {
                            sb.Append(ch);
                        }

                        break;

                    case '[':
                        if (sb.Length > 0 && sb[^1] == ch)
                        {
                            sb.Append(ch);
                            isColor = false;
                            break;
                        }
                        
                        if (isColor && sb.Length > 0)
                        {
                            yield return (color, sb.ToString());
                            color = string.Empty;
                            sb.Clear();
                        }

                        sb.Append(ch);
                        break;
                    
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case ';':
                        sb.Append(ch);
                        break;
                    
                    case 'm':
                        if (isColor && sb.Length > 1)
                        {
                            color = sb.ToString()[1..];
                            sb.Clear();
                            isColor = false;
                        }
                        else
                        {
                            sb.Append(ch);
                            isColor = false;
                        }

                        break;

                    default:
                        isColor = false;
                        sb.Append(ch);
                        break;
                }
            }
            
            yield return (color, sb.ToString());
        }
    }
}