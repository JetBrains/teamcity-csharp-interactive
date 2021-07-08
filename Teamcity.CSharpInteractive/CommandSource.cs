// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandSource : ICommandSource
    {
        private readonly ISettings _settings;
        private readonly ICommandFactory<ICodeSource> _codeSourceCommandFactory;

        public CommandSource(
            ISettings settings,
            ICommandFactory<ICodeSource> codeSourceCommandFactory)
        {
            _settings = settings;
            _codeSourceCommandFactory = codeSourceCommandFactory;
        }
        
        public IEnumerable<ICommand> GetCommands() => 
            _settings.CodeSources.SelectMany(source => _codeSourceCommandFactory.Create(source));
    }
}