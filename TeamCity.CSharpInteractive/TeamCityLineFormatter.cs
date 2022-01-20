// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace TeamCity.CSharpInteractive;

using System.Text;
using Contracts;

internal class TeamCityLineFormatter : ITeamCityLineFormatter
{
    private readonly IColorTheme _colorTheme;
        
    public TeamCityLineFormatter(IColorTheme colorTheme) => _colorTheme = colorTheme;

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    internal char EscapeSymbol { get; set; }  = '\x001B';

    public string Format(params Text[] line)
    {
        var lastColor = Color.Default;
        var sb = new StringBuilder();
        foreach (var (value, color) in line)
        {
            if (color != lastColor && !string.IsNullOrWhiteSpace(value))
            {
                sb.Append($"{EscapeSymbol}[{_colorTheme.GetAnsiColor(color)}m");
                lastColor = color;
            }

            sb.Append(value);
        }

        return sb.ToString();
    }
}