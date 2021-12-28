// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages.Write;

    internal class BuildMessageLogWriter : IBuildMessageLogWriter
    {
        private readonly ILog<BuildMessageLogWriter> _log;
        private readonly IServiceMessageFormatter _serviceMessageFormatter;

        public BuildMessageLogWriter(
            ILog<BuildMessageLogWriter> log,
            IServiceMessageFormatter serviceMessageFormatter)
        {
            _log = log;
            _serviceMessageFormatter = serviceMessageFormatter;
        }

        public void Write(BuildMessage message)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (message.State)
            {
                case BuildMessageState.ServiceMessage:
                    if(message.ServiceMessage != default)
                    {
                        _log.Trace(() => new []{ new Text(_serviceMessageFormatter.FormatMessage(message.ServiceMessage) ?? "Empty service message.") }, string.Empty);
                    }

                    break;

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