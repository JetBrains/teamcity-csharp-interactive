// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class SettingCommandFactory<TOption>: ICommandFactory<string>
        where TOption: struct, Enum
    {
        private readonly Regex _regex;

        private readonly ILog<SettingCommandFactory<TOption>> _log;
        private readonly IStringService _stringService;
        private readonly ISettingDescription _setting;

        public SettingCommandFactory(
            ILog<SettingCommandFactory<TOption>> log,
            IStringService stringService,
            IEnumerable<ISettingDescription> settingDescriptions)
        {
            _setting = settingDescriptions.SingleOrDefault(i => i.SettingType == typeof(TOption)) ?? throw new ArgumentException($"{typeof(TOption).Name} has no description of type {nameof(ISettingDescription)}.");
            _regex = new Regex($@"^#{_setting.Key}\s+(.+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            _log = log;
            _stringService = stringService;
        }
        
        public int Order => 0;

        public IEnumerable<ICommand> Create(string replCommand)
        {
            var loadMatch = _regex.Match(replCommand);
            if (!loadMatch.Success)
            {
                yield break;
            }

            var rawParam = loadMatch.Groups[1].Value;
            Enum.TryParse<TOption>(_stringService.TrimAndUnquote(rawParam), true, out var verbosityLevel);
            _log.Trace(new []{new Text($"REPL {_setting.Key}({_setting.Description}) {rawParam} -> {verbosityLevel}")});
            yield return new SettingCommand<TOption>(verbosityLevel);
        }
    }
}