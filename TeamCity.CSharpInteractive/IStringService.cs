namespace TeamCity.CSharpInteractive
{
    internal interface IStringService
    {
        string Tab { get; }

        string TrimAndUnquote(string quotedString);
    }
}