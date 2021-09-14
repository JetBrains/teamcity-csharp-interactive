// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Teamcity.CSharpInteractive
{
    using System.Text;
    using Contracts;

    internal class TeamCityLineFormatter : ITeamCityLineFormatter
    {
        private readonly IColorTheme _colorTheme;
        
        public TeamCityLineFormatter(IColorTheme colorTheme) => _colorTheme = colorTheme;

        internal char? EscapeSymbol { get; set; } = '\x001B';

        public string Format(params Text[] line)
        {
            var lastColor = Color.Default;
            var sb = new StringBuilder();
            foreach (var text in line)
            {
                if (text.Color != lastColor)
                {
                    sb.Append($"{EscapeSymbol}[{_colorTheme.GetAnsiColor(text.Color)}m");
                    lastColor = text.Color;
                }

                sb.Append(text.Value);
            }

            return sb.ToString();
        }
    }
}