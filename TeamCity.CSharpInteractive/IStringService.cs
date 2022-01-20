namespace TeamCity.CSharpInteractive;

internal interface IStringService
{
    string TrimAndUnquote(string quotedString);
}