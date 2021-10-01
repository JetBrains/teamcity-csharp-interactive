// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityLog<T>: ILog<T>
    {
        private readonly ISettings _settings;
        private readonly ITeamCityLineFormatter _lineFormatter;
        private readonly ITeamCityBlockWriter<ITeamCityWriter> _blockWriter;
        private readonly ITeamCityMessageWriter _teamCityMessageWriter;
        private readonly ITeamCityBuildStatusWriter _teamCityBuildStatusWriter;
        private readonly IStatistics _statistics;

        public TeamCityLog(
            ISettings settings,
            ITeamCityLineFormatter lineFormatter,
            ITeamCityBlockWriter<ITeamCityWriter> blockWriter,
            ITeamCityMessageWriter teamCityMessageWriter,
            ITeamCityBuildStatusWriter teamCityBuildStatusWriter,
            IStatistics statistics)
        {
            _settings = settings;
            _lineFormatter = lineFormatter;
            _blockWriter = blockWriter;
            _teamCityMessageWriter = teamCityMessageWriter;
            _teamCityBuildStatusWriter = teamCityBuildStatusWriter;
            _statistics = statistics;
        }

        public void Error(ErrorId id, params Text[] error)
        {
            var message = error.ToSimpleString();
            _statistics.RegisterError(message);
            _teamCityBuildStatusWriter.WriteBuildProblem(id.Id, message);
        }

        public void Warning(params Text[] warning)
        {
            var message = warning.ToSimpleString();
            _statistics.RegisterWarning(message);
            _teamCityMessageWriter.WriteWarning(message);
        }

        public void Info(params Text[] message)
        {
            if (_settings.VerbosityLevel >= VerbosityLevel.Normal)
            {
                _teamCityMessageWriter.WriteMessage(_lineFormatter.Format(message));
            }
        }

        public void Trace(string origin, params Text[] traceMessage)
        {
            // ReSharper disable once InvertIf
            if (_settings.VerbosityLevel >= VerbosityLevel.Diagnostic)
            {
                origin = string.IsNullOrWhiteSpace(origin) ? typeof(T).Name : origin.Trim();
                _teamCityMessageWriter.WriteMessage(_lineFormatter.Format((new Text($"{origin, -40}") + traceMessage).WithDefaultColor(Color.Trace)));
            }
        }

        public IDisposable Block(Text[] block) => _blockWriter.OpenBlock(block.ToSimpleString());
    }
}