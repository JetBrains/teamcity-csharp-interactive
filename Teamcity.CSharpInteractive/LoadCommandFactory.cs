// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class LoadCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex Regex = new(@"^#load\s+""(.+)""$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly ILog<LoadCommandFactory> _log;
        private readonly IFileCodeSourceFactory _fileCodeSourceFactory;
        private readonly Func<ICommandFactory<ICodeSource>> _codeSourceCommandFactory;

        public LoadCommandFactory(
            ILog<LoadCommandFactory> log,
            IFileCodeSourceFactory fileCodeSourceFactory,
            Func<ICommandFactory<ICodeSource>> codeSourceCommandFactory)
        {
            _log = log;
            _fileCodeSourceFactory = fileCodeSourceFactory;
            _codeSourceCommandFactory = codeSourceCommandFactory;
        }

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var loadMatch = Regex.Match(replCommand);
            if (!loadMatch.Success)
            {
                return Enumerable.Empty<ICommand>();
            }

            var path = loadMatch.Groups[1].Value;
            var fileSource = _fileCodeSourceFactory.Create(path);
            return _codeSourceCommandFactory().Create(fileSource);
        }
    }
}