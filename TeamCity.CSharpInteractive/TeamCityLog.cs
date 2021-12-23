// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityLog<T>: ILog<T>
    {
        private readonly ISettings _settings;
        private readonly ITeamCityWriter _teamCityWriter;
        private readonly ITeamCityLineFormatter _lineFormatter;
        private readonly IStatistics _statistics;

        public TeamCityLog(
            ISettings settings,
            ITeamCityWriter teamCityWriter,
            ITeamCityLineFormatter lineFormatter,
            IStatistics statistics)
        {
            _settings = settings;
            _teamCityWriter = teamCityWriter;
            _lineFormatter = lineFormatter;
            _statistics = statistics;
        }

        public void Error(ErrorId id, params Text[] error)
        {
            var message = error.ToSimpleString();
            _statistics.RegisterError(message);
            _teamCityWriter.WriteBuildProblem(id.Id, message);
        }

        public void Warning(params Text[] warning)
        {
            var message = warning.ToSimpleString();
            _statistics.RegisterWarning(message);
            _teamCityWriter.WriteWarning(message);
        }

        public void Info(params Text[] message)
        {
            if (_settings.VerbosityLevel >= VerbosityLevel.Normal)
            {
                _teamCityWriter.WriteMessage(_lineFormatter.Format(message));
            }
        }

        public void Trace(Func<Text[]> traceMessagesFactory, string origin)
        {
            // ReSharper disable once InvertIf
            if (_settings.VerbosityLevel >= VerbosityLevel.Diagnostic)
            {
                origin = string.IsNullOrWhiteSpace(origin) ? typeof(T).Name : origin.Trim();
                _teamCityWriter.WriteMessage(_lineFormatter.Format((new Text($"{origin, -40}") + traceMessagesFactory()).WithDefaultColor(Color.Trace)));
            }
        }

        public IDisposable Block(Text[] block) => _teamCityWriter.OpenBlock(block.ToSimpleString());
    }
}