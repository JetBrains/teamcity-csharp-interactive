namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Shouldly;
    using Xunit;

    public class TextToColorStringsTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldConvert(string text, (ConsoleColor? color, string text)[] expected)
        {
            // Given
            var instance = CreateInstance();

            // When
            var actual = instance.Convert(text, ConsoleColor.Blue).ToArray();

            // Then
            actual.ShouldBe(expected);
        }

        private static TextToColorStrings CreateInstance() =>
            new();
        
        public static IEnumerable<object[]> Data => new List<object[]> 
        {
            new object[] { "Usage: dotnet csi [options] [script-file.csx] [script-arguments]", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Usage: dotnet csi [options] [script-file.csx] [script-arguments]")} },
            new object[] { "\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "[36mAbc")} },
            new object[] { "Abc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Abc")} },
            new object[] { "", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "")} },
            new object[] { "Xyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[93mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Yellow, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[aamXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[aa;36mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[1;36mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[36;1mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} }
        };
    }
}