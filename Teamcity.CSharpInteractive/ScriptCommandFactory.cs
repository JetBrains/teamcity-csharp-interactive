// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    internal class ScriptCommandFactory : IScriptCommandFactory
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

        public bool HasCode => _scriptBuilder.Length > 0;

        public ICommand Create(string originName, string code)
        {
            _scriptBuilder.AppendLine(code);
            var script = _scriptBuilder.ToString();
            if (_scriptSubmissionAnalyzer.IsCompleteSubmission(script, ParseOptions))
            {
                _log.Trace(new []{new Text("Completed submission")});
                _scriptBuilder.Clear();
                return new ScriptCommand(originName, script);
            }

            _log.Trace(new []{new Text("Incomplete submission")});
            return CodeCommand.Shared;
        }
    }
}