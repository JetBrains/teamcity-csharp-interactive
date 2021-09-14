// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using Contracts;

    internal class ScriptHost : IHost
    {
        private readonly ILog<ScriptHost> _log;
        private readonly IStdOut _stdOut;

        public ScriptHost(
            ILog<ScriptHost> log,
            IStdOut stdOut)
        {
            _log = log;
            _stdOut = stdOut;
        }

        public IHost Host => this;

        public void WriteLine() => _stdOut.WriteLine();

        public void WriteLine<T>(T line, Color color = Color.Default) => _stdOut.WriteLine(new Text(line?.ToString() ?? string.Empty, color));

        public void Error(string error, string errorId = "Unknown") => _log.Error(new ErrorId(errorId), error);

        public void Warning(string warning) => _log.Warning(warning);

        public void Info(string text) => _log.Info(text);

        public void Trace(string trace, string origin = "") => _log.Trace(origin, trace);
    }
}