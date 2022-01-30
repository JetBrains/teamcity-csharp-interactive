namespace TeamCity.CSharpInteractive;

internal interface ICommand
{
    string Name { get; }

    bool Internal { get; }
}