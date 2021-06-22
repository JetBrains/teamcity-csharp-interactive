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
        
        public IEnumerable<ICommand> GetCommands()
        {
            foreach (var source in _settings.Sources)
            {
                foreach (var command in _codeSourceCommandFactory.Create(source))
                {
                    yield return command;
                }
            }
        }
    }
}