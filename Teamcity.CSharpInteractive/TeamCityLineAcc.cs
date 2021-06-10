// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class TeamCityLineAcc : ITeamCityLineAcc
    {
        private readonly IColorTheme _colorTheme;
        private readonly List<Text> _text = new();

        public TeamCityLineAcc(IColorTheme colorTheme) => _colorTheme = colorTheme;

        internal char? EscapeSymbol { get; set; } = '\x001B';

        public void Write(params Text[] text)
        {
            foreach (var textItem in text)
            {
                if (Text.NewLine.Equals(textItem))
                {
                    _text.Add(textItem);
                    continue;
                }
                
                var lines = textItem.Value.Split(System.Environment.NewLine);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    _text.Add(new Text(line, textItem.Color));
                    if (i < lines.Length - 1)
                    {
                        _text.Add(Text.NewLine);
                    }
                }
            }
        }

        public IEnumerable<string> GetLines(bool includingIncomplete)
        {
            if (!_text.Any())
            {
                yield break;
            }
            
            Color? latsColor = null;
            var line = new StringBuilder();
            var processedItems = 0;
            var counter = 0;
            foreach (var text in _text)
            {
                counter++;
                if (Text.NewLine.Equals(text))
                {
                    processedItems = counter;
                    yield return line.ToString();
                    line.Clear();
                    latsColor = null;
                    continue;
                }
                
                if (text.Color != latsColor && text.Color != Color.Default)
                {
                    line.Append($"{EscapeSymbol}[{_colorTheme.GetAnsiColor(text.Color)}m");
                    latsColor = text.Color;
                }

                line.Append(text.Value);
            }

            if (includingIncomplete)
            {
                yield return line.ToString();
                _text.Clear();
            }
            else
            {
                while (processedItems-- > 0)
                {
                    _text.RemoveAt(0);
                }
            }
        }
    }
}