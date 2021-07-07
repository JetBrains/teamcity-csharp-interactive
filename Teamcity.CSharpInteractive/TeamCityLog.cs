// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityLog<T>: ILog<T>
    {
        private readonly ISettings _settings;
        private readonly ITeamCityLineFormatter _lineFormatter;
        private readonly ITeamCityBlockWriter<IDisposable> _blockWriter;
        private readonly ITeamCityMessageWriter _teamCityMessageWriter;
        private readonly ITeamCityBuildProblemWriter _teamCityBuildProblemWriter;
        private readonly IStatistics _statistics;

        public TeamCityLog(
            ISettings settings,
            ITeamCityLineFormatter lineFormatter,
            ITeamCityBlockWriter<IDisposable> blockWriter,
            ITeamCityMessageWriter teamCityMessageWriter,
            ITeamCityBuildProblemWriter teamCityBuildProblemWriter,
            IStatistics statistics)
        {
            _settings = settings;
            _lineFormatter = lineFormatter;
            _blockWriter = blockWriter;
            _teamCityMessageWriter = teamCityMessageWriter;
            _teamCityBuildProblemWriter = teamCityBuildProblemWriter;
            _statistics = statistics;
        }

        public void Error(ErrorId id, params Text[] error)
        {
            var message = error.ToSimpleString();
            _statistics.RegisterError(message);
            _teamCityBuildProblemWriter.WriteBuildProblem(id.Id, message);
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

        public void Trace(params Text[] traceMessage)
        {
            if (_settings.VerbosityLevel >= VerbosityLevel.Trace)
            {
                _teamCityMessageWriter.WriteMessage(_lineFormatter.Format(traceMessage.WithDefaultColor(Color.Trace)));
            }
        }

        public IDisposable Block(Text[] block) => _blockWriter.OpenBlock(block.ToSimpleString());
    }
}