// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Linq;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityLog<T>: ILog<T>
    {
        private readonly ISettings _settings;
        private readonly Func<ITeamCityLineAcc> _teamCityLineAccFactory;
        private readonly ITeamCityBlockWriter<IDisposable> _blockWriter;
        private readonly ITeamCityMessageWriter _teamCityMessageWriter;
        private readonly ITeamCityBuildProblemWriter _teamCityBuildProblemWriter;
        private readonly IStatistics _statistics;

        public TeamCityLog(
            ISettings settings,
            Func<ITeamCityLineAcc> teamCityLineAccFactory,
            ITeamCityBlockWriter<IDisposable> blockWriter,
            ITeamCityMessageWriter teamCityMessageWriter,
            ITeamCityBuildProblemWriter teamCityBuildProblemWriter,
            IStatistics statistics)
        {
            _settings = settings;
            _teamCityLineAccFactory = teamCityLineAccFactory;
            _blockWriter = blockWriter;
            _teamCityMessageWriter = teamCityMessageWriter;
            _teamCityBuildProblemWriter = teamCityBuildProblemWriter;
            _statistics = statistics;
        }

        public void Error(ErrorId id, params Text[] error)
        {
            _statistics.RegisterError(string.Join("", error.Select(i => i.Value)));
            WriteLines(error, i => _teamCityBuildProblemWriter.WriteBuildProblem(id.Id,i));
        }

        public void Warning(params Text[] warning)
        {
            _statistics.RegisterWarning(string.Join("", warning.Select(i => i.Value)));
            WriteLines(warning, i => _teamCityMessageWriter.WriteWarning(i));
        }

        public void Info(params Text[] message)
        {
            if (_settings.VerbosityLevel >= VerbosityLevel.Normal)
            {
                WriteLines(message, i => _teamCityMessageWriter.WriteMessage(i));
            }
        }

        public void Trace(params Text[] traceMessage)
        {
            if (_settings.VerbosityLevel >= VerbosityLevel.Trace)
            {
                WriteLines(traceMessage.WithDefaultColor(Color.Trace), i => _teamCityMessageWriter.WriteMessage(i));
            }
        }

        public IDisposable Block(Text[] block)
        {
            var acc = _teamCityLineAccFactory();
            acc.Write(block);
            return _blockWriter.OpenBlock(string.Join("", block.Select(i => i.Value)));
        }
        
        private void WriteLines(Text[] text, Action<string> lineWriter)
        {
            var acc = _teamCityLineAccFactory();
            acc.Write(text);
            foreach (var line in acc.GetLines(true))
            {
                lineWriter(line);
            }
        }
    }
}