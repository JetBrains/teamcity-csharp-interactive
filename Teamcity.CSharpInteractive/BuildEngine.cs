// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using Contracts;
    using Microsoft.Build.Framework;

    [ExcludeFromCodeCoverage]
    internal class BuildEngine : IBuildEngine
    {
        private readonly ILog<BuildEngine> _log;

        public BuildEngine(ILog<BuildEngine> log) => _log = log;

        public void LogErrorEvent(BuildErrorEventArgs e) => _log.Error(new ErrorId(e.Code), new []{new Text(e.Message)});

        public void LogWarningEvent(BuildWarningEventArgs e) => _log.Warning(new []{new Text(e.Message)});

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.Importance)
            {
                case MessageImportance.High:
                    _log.Info(new[] {new Text(e.Message, Color.Header)});
                    break;
                
                case MessageImportance.Normal:
                    _log.Info(new[] {new Text(e.Message, Color.Details)});
                    break;
                
                default:
                    _log.Trace(new[] {new Text(e.Message)});
                    break;
            }
        }

        public void LogCustomEvent(CustomBuildEventArgs e) => _log.Trace(new []{new Text(e.Message)});

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) =>
            true;

        public bool ContinueOnError => true;

        public int LineNumberOfTaskNode => 0;

        public int ColumnNumberOfTaskNode => 0;

        public string ProjectFileOfTaskNode => "Restore";
    }
}