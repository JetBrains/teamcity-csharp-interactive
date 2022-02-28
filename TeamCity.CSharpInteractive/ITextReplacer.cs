namespace TeamCity.CSharpInteractive;

internal interface ITextReplacer
{
    Stream Replace(Stream source, Func<IEnumerable<string>, IEnumerable<string>> replacer);
}