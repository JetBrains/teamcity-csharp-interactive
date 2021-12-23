// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class FileCodeSource: ICodeSource
    {
        private readonly ILog<FileCodeSource> _log;
        private readonly IFileTextReader _fileTextReader;
        private readonly IFilePathResolver _filePathResolver;
        private readonly IScriptContext _scriptContext;
        private string _fileName = "";

        public FileCodeSource(
            ILog<FileCodeSource> log,
            IFileTextReader fileTextReader,
            IFilePathResolver filePathResolver,
            IScriptContext scriptContext)
        {
            _log = log;
            _fileTextReader = fileTextReader;
            _filePathResolver = filePathResolver;
            _scriptContext = scriptContext;
        }

        public string Name => Path.GetFileName(FileName);
        
        public bool Internal => false;

        public string FileName
        {
            get => _fileName;
            set
            {
                if (!_filePathResolver.TryResolve(value, out var fullFilePath))
                {
                    fullFilePath = value;
                }
                
                _fileName = fullFilePath;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            var resource = _scriptContext.OverrideScriptDirectory(Path.GetDirectoryName(FileName));
            try
            {
                _log.Trace(() => new []{new Text($@"Read file ""{FileName}"".")});
                return new LinesEnumerator(_fileTextReader.ReadLines(FileName).GetEnumerator(), () => resource.Dispose());
            }
            catch (Exception e)
            {
                _log.Error(ErrorId.File, new []{new Text(e.Message)});
                return Enumerable.Empty<string>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        private class LinesEnumerator: IEnumerator<string>
        {
            private readonly IEnumerator<string> _baseEnumerator;
            private readonly Action _onDispose;

            public LinesEnumerator(IEnumerator<string> baseEnumerator, Action onDispose)
            {
                _baseEnumerator = baseEnumerator;
                _onDispose = onDispose;
            }

            public bool MoveNext() => _baseEnumerator.MoveNext();

            public void Reset() => _baseEnumerator.Reset();

            public string Current => _baseEnumerator.Current;

            object? IEnumerator.Current => ((IEnumerator) _baseEnumerator).Current;

            public void Dispose()
            {
                try
                {
                    _baseEnumerator.Dispose();
                }
                finally
                {
                    _onDispose();
                }
            }
        }
    }
}