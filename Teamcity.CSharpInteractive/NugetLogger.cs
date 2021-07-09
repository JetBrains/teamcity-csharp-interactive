// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using NuGet.Common;

    [ExcludeFromCodeCoverage]
    internal class NugetLogger: ILogger
    {
        private readonly ILog<NugetLogger> _log;

        public NugetLogger(ILog<NugetLogger> log) => _log = log;

        public void LogDebug(string data) => _log.Trace(new []{new Text(data)});

        public void LogVerbose(string data) => _log.Trace(new []{new Text(data)});

        public void LogInformation(string data) => _log.Info(new []{new Text(data)});

        public void LogMinimal(string data) => _log.Info(new []{new Text(data)});

        public void LogWarning(string data) => _log.Warning(new []{new Text(data)});

        public void LogError(string data) => _log.Error(ErrorId.Nuget, new []{new Text(data)});

        public void LogInformationSummary(string data) => _log.Trace(new []{new Text(data)});

        public void Log(LogLevel level, string data)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;
                
                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
                
                case LogLevel.Information:
                    LogInformation(data);
                    break;
                
                case LogLevel.Minimal:
                    LogMinimal(data);
                    break;
                
                case LogLevel.Warning:
                    LogWarning(data);
                    break;
                
                case LogLevel.Error:
                    LogError(data);
                    break;
            }
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public void Log(ILogMessage message) => Log(message.Level, message.Message);

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}