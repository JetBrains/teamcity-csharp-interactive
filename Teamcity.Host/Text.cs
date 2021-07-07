namespace Teamcity.Host
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal readonly struct Text
    {
        public readonly string Value;
        public readonly Color Color;

        public static readonly Text NewLine = new(System.Environment.NewLine);

        public Text(string value, Color color = Color.Default)
        {
            Value = value;
            Color = color;
        }

        public static implicit operator Text[](Text text) => new[] {text};

        public override string ToString() => $"{Color}:{Value}";

        public static Text[] operator +(Text[] text, Text text2)
        {
            var newText = new Text[text.Length + 1];
            Array.Copy(text, 0, newText, 0, text.Length);
            newText[text.Length] = text2;
            return newText;
        }

        public static Text[] operator +(Text text1, Text[] text)
        {
            var newText = new Text[text.Length + 1];
            newText[0] = text1;
            Array.Copy(text, 0, newText, 1, text.Length);
            return newText;
        }
    }
}