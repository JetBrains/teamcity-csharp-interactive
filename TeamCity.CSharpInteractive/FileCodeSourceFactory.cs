// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class FileCodeSourceFactory : IFileCodeSourceFactory
{
    private readonly Func<FileCodeSource> _fileCodeSourceFactory;

    public FileCodeSourceFactory(Func<FileCodeSource> fileCodeSourceFactory) =>
        _fileCodeSourceFactory = fileCodeSourceFactory;

    public ICodeSource Create(string fileName)
    {
        var fileCodeSource = _fileCodeSourceFactory();
        fileCodeSource.Name = fileName;
        return fileCodeSource;
    }
}