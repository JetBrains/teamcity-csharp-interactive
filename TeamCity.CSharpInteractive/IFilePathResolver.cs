namespace TeamCity.CSharpInteractive;

internal interface IFilePathResolver
{
    bool TryResolve(string? fileOrDirectoryPath, out string fullScriptPath);
}