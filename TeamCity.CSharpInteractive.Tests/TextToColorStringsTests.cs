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
        [MemberData(nameof(ConvertData))]
        public void ShouldConvert(string text, (ConsoleColor? color, string text)[] expected)
        {
            // Given
            var instance = CreateInstance();

            // When
            var actual = instance.Convert(text, ConsoleColor.Blue).ToArray();

            // Then
            actual.ShouldBe(expected);
        }

        public static IEnumerable<object[]> ConvertData => new List<object[]> 
        {
            new object[] { "Usage: dotnet csi [options] [script-file.csx] [script-arguments]", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Usage: dotnet csi [options] [script-file.csx] [script-arguments]")} },
            new object[] { "\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "[36mAbc")} },
            new object[] { "Abc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Abc")} },
            new object[] { "\x001BAbcmXyz", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "\x001BAbcmXyz")} },
            new object[] { "\x001B[AbcmXyz", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "\x001B[AbcmXyz")} },
            new object[] { "", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "")} },
            new object[] { "Xyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[93mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Yellow, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "\x001B[mXyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[aamXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "\x001B[aamXyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[aa;36mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.Blue, "\x001B[aa;36mXyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[1;36mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} },
            new object[] { "\x001B[36;1mXyz\x001B[36mAbc", new (ConsoleColor?, string)[] {(ConsoleColor.DarkCyan, "Xyz"), (ConsoleColor.DarkCyan, "Abc")} }
        };
        
        [Theory]
        [MemberData(nameof(SplitData))]
        public void ShouldSplit(string text, (string color, string text)[] expected)
        {
            // Given
            
            // When
            var actual = TextToColorStrings.Split(text).ToArray();

            // Then
            actual.ShouldBe(expected);
        }

        public static IEnumerable<object[]> SplitData => new List<object[]> 
        {
            new object[] { "\x001B[93mXyz\x001B[36mAbc", new[] {("93", "Xyz"), ("36", "Abc")} },
            new object[] { "\x001B[93mXyz", new[] {("93", "Xyz")} },
            new object[] { "\x001B[93;22;44mXyz", new[] {("93;22;44", "Xyz")} },
            new object[] { "", new[] {("", "")} },
            new object[] { "   ", new[] {("", "   ")} },
            new object[] { "mmm", new[] {("", "mmm")} },
            new object[] { "##teamcity[[[", new[] {("", "##teamcity[[[")} },
            new object[] { "##teamcity[message text='\x001B|[34;1m Copying file from']", new[] {("", "##teamcity[message text='\x001B|[34;1m Copying file from']")} },
            new object[] { "JetBrains TeamCity C# script runner 1.0.0-dev net6.0", new[] {("", "JetBrains TeamCity C# script runner 1.0.0-dev net6.0")} },
            new object[] { "\x001B|[93mXyz", new[] {("", "\x001B|[93mXyz")} },
            new object[] { "\x001B[93amXyz", new[] {("", "\x001B[93amXyz")} },
            new object[] { "[93mXyz", new[] {("", "[93mXyz")} },
            new object[] { "\x001B[mXyz", new[] {("", "\x001B[mXyz")} },
            new object[] { "\x001B\x001B[93mXyz", new[] {("", "\x001B"), ("93", "Xyz")} },
            new object[] { "\x001B\x001B[mXyz", new[] {("", "\x001B"), ("", "\x001B\x001B[mXyz")} },
            new object[] { "\x001B[[93mXyz", new[] {("", "\x001B[[93mXyz")} },
            new object[] { "\x001B[[[93mXyz", new[] {("", "\x001B[[[93mXyz")} },
        };
        
        private static TextToColorStrings CreateInstance() =>
            new();
    }
}