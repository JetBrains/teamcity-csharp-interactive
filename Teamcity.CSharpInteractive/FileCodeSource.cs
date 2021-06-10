// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
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

        public FileCodeSource(
            ILog<FileCodeSource> log,
            IFileTextReader fileTextReader)
        {
            _log = log;
            _fileTextReader = fileTextReader;
        }

        public string Name => Path.GetFileName(FileName);

        public bool ResetRequired => true;

        public string FileName { get; set; } = "";
        
        public IEnumerator<string> GetEnumerator()
        {
            try
            {
                return Enumerable.Repeat(_fileTextReader.Read(FileName), 1).GetEnumerator();
            }
            catch (Exception e)
            {
                _log.Error(ErrorId.File, new []{new Text(e.Message)});
                return Enumerable.Empty<string>().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}