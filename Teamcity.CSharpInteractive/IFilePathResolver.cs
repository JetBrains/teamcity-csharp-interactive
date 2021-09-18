namespace TeamCity.CSharpInteractive
{
    internal interface IFilePathResolver
    {
        bool TryResolve(string? filePath, out string fullFilePath);
    }
}