// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class LoadCommandFactory: ICommandFactory<string>
    {
        private static readonly Regex Regex = new(@"^\s*#load\s+""(.+)""\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly IFileCodeSourceFactory _fileCodeSourceFactory;
        private readonly Func<ICommandFactory<ICodeSource>> _codeSourceCommandFactory;

        public LoadCommandFactory(
            IFileCodeSourceFactory fileCodeSourceFactory,
            Func<ICommandFactory<ICodeSource>> codeSourceCommandFactory)
        {
            _fileCodeSourceFactory = fileCodeSourceFactory;
            _codeSourceCommandFactory = codeSourceCommandFactory;
        }
        
        public int Order => 0;

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