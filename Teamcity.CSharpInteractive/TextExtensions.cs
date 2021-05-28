namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal static class TextExtensions
    {
        public static Text[] WithDefaultColor(this Text[] text, Color color)
        {
            var newText = new Text[text.Length];
            for (var i = 0; i < newText.Length; i++)
            {
                var curText = text[i];
                newText[i] = new Text(curText.Value, curText.Color == Color.Default ? color : curText.Color);
            }

            return newText;
        }
    }
}