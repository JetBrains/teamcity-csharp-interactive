// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;

    internal class FileCodeSourceFactory : IFileCodeSourceFactory
    {
        private readonly Func<FileCodeSource> _fileCodeSourceFactory;

        public FileCodeSourceFactory(Func<FileCodeSource> fileCodeSourceFactory) => 
            _fileCodeSourceFactory = fileCodeSourceFactory;

        public ICodeSource Create(string fileName)
        {
            var fileCodeSource = _fileCodeSourceFactory();
            fileCodeSource.FileName = fileName;
            return fileCodeSource;
        }
    }
}