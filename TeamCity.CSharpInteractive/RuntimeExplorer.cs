// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

internal class RuntimeExplorer : IRuntimeExplorer
{
    private readonly string _runtimePath;
    private readonly IFileSystem _fileSystem;
    public RuntimeExplorer(
        [Tag("RuntimePath")] string runtimePath,
        IFileSystem fileSystem)
    {
        _runtimePath = runtimePath;
        _fileSystem = fileSystem;
    }

    public bool TryFindRuntimeAssembly(string assemblyPath, [MaybeNullWhen(false)] out string runtimeAssemblyPath)
    {
        if (string.IsNullOrWhiteSpace(_runtimePath))
        {
            runtimeAssemblyPath = default;
            return false;
        }

        runtimeAssemblyPath = _fileSystem.EnumerateFileSystemEntries(_runtimePath, Path.GetFileName(assemblyPath), SearchOption.TopDirectoryOnly).FirstOrDefault();
        return runtimeAssemblyPath != default;
    }
}