// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal class ScriptCommandFactory : ICommandFactory<ScriptCommand>
    {
        internal static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest, kind: SourceCodeKind.Script);
        private readonly ILog<ScriptCommandFactory> _log;
        private readonly IScriptSubmissionAnalyzer _scriptSubmissionAnalyzer;
        private readonly StringBuilder _scriptBuilder = new();

        public ScriptCommandFactory(
            ILog<ScriptCommandFactory> log,
            IScriptSubmissionAnalyzer scriptSubmissionAnalyzer)
        {
            _log = log;
            _scriptSubmissionAnalyzer = scriptSubmissionAnalyzer;
        }

        public int Order => 0;

        public IEnumerable<ICommand> Create(ScriptCommand scriptCommand)
        {
            _scriptBuilder.AppendLine(scriptCommand.Script);
            var script = _scriptBuilder.ToString();
            if (_scriptSubmissionAnalyzer.IsCompleteSubmission(script, ParseOptions))
            {
                _log.Trace(new []{new Text("Completed submission")});
                _scriptBuilder.Clear();
                yield return new ScriptCommand(scriptCommand.Name, script, scriptCommand.Internal);
                yield break;
            }

            _log.Trace(new []{new Text("Incomplete submission")});
            yield return new CodeCommand(scriptCommand.Internal);
        }
    }
}