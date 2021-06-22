// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class FileCodeSourceFactory : IFileCodeSourceFactory
    {
        private readonly Func<FileCodeSource> _fileCodeSourceFactory;

        public FileCodeSourceFactory(Func<FileCodeSource> fileCodeSourceFactory) => 
            _fileCodeSourceFactory = fileCodeSourceFactory;

        public ICodeSource Create(string fileName, bool isRoot)
        {
            var fileCodeSource = _fileCodeSourceFactory();
            fileCodeSource.FileName = fileName;
            fileCodeSource.ResetRequired = isRoot;
            return fileCodeSource;
        }
    }
}