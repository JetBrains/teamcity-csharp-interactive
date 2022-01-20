namespace TeamCity.CSharpInteractive;

internal interface ICodeSource : IEnumerable<string?>
{
    string Name { get; }
        
    bool Internal { get; }
}