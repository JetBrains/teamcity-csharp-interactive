// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Host;

    internal class FileCodeSource: ICodeSource
    {
        private readonly ILog<FileCodeSource> _log;
        private readonly IFileTextReader _fileTextReader;
        private readonly IEnvironment _environment;
        private readonly IWorkingDirectoryContext _workingDirectoryContext;
        private string _fileName = "";

        public FileCodeSource(
            ILog<FileCodeSource> log,
            IFileTextReader fileTextReader,
            IEnvironment environment,
            IWorkingDirectoryContext workingDirectoryContext)
        {
            _log = log;
            _fileTextReader = fileTextReader;
            _environment = environment;
            _workingDirectoryContext = workingDirectoryContext;
        }

        public string Name => Path.GetFileName(FileName);
        
        public bool Internal => false;

        public string FileName
        {
            get => _fileName;
            set => _fileName = Path.IsPathRooted(value) ? value : Path.Combine(_environment.GetPath(SpecialFolder.WorkingDirectory), value);
        }

        public IEnumerator<string> GetEnumerator()
        {
            var resource = _workingDirectoryContext.OverrideWorkingDirectory(Path.GetDirectoryName(FileName));
            try
            {
                _log.Trace($@"Read file ""{FileName}"".");
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