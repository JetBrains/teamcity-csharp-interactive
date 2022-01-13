// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages.Write;

    internal class BuildMessageLogWriter : IBuildMessageLogWriter
    {
        private readonly ILog<BuildMessageLogWriter> _log;

        public BuildMessageLogWriter(
            ILog<BuildMessageLogWriter> log)
        {
            _log = log;
        }

        public void Write(BuildMessage message)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (message.State)
            {
                case BuildMessageState.Info:
                    _log.Info(message.Text);
                    break;

                case BuildMessageState.Warning:
                    _log.Warning(message.Text);
                    break;

                case BuildMessageState.Failure:
                case BuildMessageState.Error:
                case BuildMessageState.BuildProblem:
                    _log.Error(ErrorId.Build, message.Text);
                    break;
            }
        }
    }
}