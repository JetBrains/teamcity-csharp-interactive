namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Contracts;

    [ExcludeFromCodeCoverage]
    internal readonly record struct Text(string Value, Color Color = Color.Default)
    {
        public static readonly Text Empty = new(string.Empty);
        public static readonly Text NewLine = new(System.Environment.NewLine);
        public static readonly Text Space = new(" ");
        public static readonly Text Tab = new("    ");
        
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