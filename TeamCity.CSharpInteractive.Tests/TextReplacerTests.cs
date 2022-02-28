namespace TeamCity.CSharpInteractive.Tests;

using System;
using Xunit;

public class TextReplacerTests
{
    [Fact]
    public void ShouldReplace()
    {
        // Given
        var replacer = CreateInstance();
        var source = new MemoryStream();
        using var writer = new StreamWriter(source);
        writer.WriteLine("Abc");
        writer.WriteLine("###");
        writer.WriteLine("Xyz");
        writer.Flush();
        source.Position = 0;

        // When
        var replacedStream = replacer.Replace(source, sourceLines => sourceLines.Select(i => i.Replace("###", "!!!")));

        // Then
        using var reader = new System.IO.StreamReader(replacedStream);
        var result = reader.ReadToEnd();
        result.ShouldBe($"Abc{Environment.NewLine}!!!{Environment.NewLine}Xyz{Environment.NewLine}");
    }

    private static TextReplacer CreateInstance() =>
        new();
}