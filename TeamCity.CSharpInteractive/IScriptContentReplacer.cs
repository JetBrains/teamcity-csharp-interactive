namespace TeamCity.CSharpInteractive;

internal interface IScriptContentReplacer
{
    IEnumerable<string> Replace(IEnumerable<string> lines);
}