namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Contracts;

    [ExcludeFromCodeCoverage]
    internal readonly record struct Text(string Value, Color Color)
    {
        public static readonly Text Empty = new(string.Empty);
        public static readonly Text NewLine = new(System.Environment.NewLine);
        public static readonly Text Space = new(" ");
        public static readonly Text Tab = new("    ");

        public Text(string value)
            : this(value, Color.Default)
        { }

        public static implicit operator Text[](Text text) => new[] {text};

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Value);
            // ReSharper disable once InvertIf
            if (Color != Color.Default)
            {
                sb.Append('[');
                sb.Append(Color);
                sb.Append(']');
            }
            
            return sb.ToString();
        }

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