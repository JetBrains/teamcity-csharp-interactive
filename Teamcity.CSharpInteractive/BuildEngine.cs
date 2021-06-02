// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections;
    using Microsoft.Build.Framework;

    internal class BuildEngine : IBuildEngine
    {
        private readonly ILog<BuildEngine> _log;

        public BuildEngine(ILog<BuildEngine> log) => _log = log;

        public void LogErrorEvent(BuildErrorEventArgs e) => _log.Error(new []{new Text(e.Message)});

        public void LogWarningEvent(BuildWarningEventArgs e) => _log.Warning(new []{new Text(e.Message)});

        public void LogMessageEvent(BuildMessageEventArgs e) => _log.Trace(new []{new Text(e.Message)});

        public void LogCustomEvent(CustomBuildEventArgs e) => _log.Trace(new []{new Text(e.Message)});

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs) =>
            true;

        public bool ContinueOnError => true;

        public int LineNumberOfTaskNode => 1;

        public int ColumnNumberOfTaskNode => 1;

        public string ProjectFileOfTaskNode => "Restore";
    }
}