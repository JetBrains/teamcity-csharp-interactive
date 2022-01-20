namespace TeamCity.CSharpInteractive;

internal interface IExitCodeParser
{
    bool TryParse(object returnValue, out int exitCode);
}