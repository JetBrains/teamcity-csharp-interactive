// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Dotnet;

    internal class BuildMessageLogWriter : IBuildMessageLogWriter
    {
        private readonly ILog<BuildMessageLogWriter> _log;

        public BuildMessageLogWriter(ILog<BuildMessageLogWriter> log) => _log = log;

        public void Write(BuildMessage message)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (message.State)
            {
                case BuildMessageState.ServiceMessage:
                    if(message.ServiceMessage != default)
                    {
                        _log.Trace(() => new []{ new Text(message.ServiceMessage.ToString() ?? "Empty service message.") }, string.Empty);
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