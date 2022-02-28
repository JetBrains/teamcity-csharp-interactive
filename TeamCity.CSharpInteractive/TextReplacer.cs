// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class TextReplacer : ITextReplacer
{
    public Stream Replace(Stream source, Func<IEnumerable<string>, IEnumerable<string>> replacer)
    {
        using var reader = new System.IO.StreamReader(source);
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        foreach (var line in replacer(ReadLines(reader)))
        {
            writer.WriteLine(line);
        }

        writer.Flush();
        ms.Position = 0;
        return ms;
    }
    
    private static IEnumerable<string> ReadLines(TextReader reader)
    {
        string? line;
        do
        {
            line = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
        while(line != default);
    }
}