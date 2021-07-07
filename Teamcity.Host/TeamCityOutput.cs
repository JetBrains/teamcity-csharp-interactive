// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityOutput : IStdOut, IStdErr
    {
        private readonly ITeamCityLineFormatter _lineFormatter;
        private readonly ITeamCityMessageWriter _teamCityMessageWriter;

        public TeamCityOutput(
            ITeamCityLineFormatter lineFormatter,
            ITeamCityMessageWriter teamCityMessageWriter)
        {
            _lineFormatter = lineFormatter;
            _teamCityMessageWriter = teamCityMessageWriter;
        }

        public void Write(params Text[] text) => _teamCityMessageWriter.WriteMessage(_lineFormatter.Format(text));

        void IStdOut.WriteLine(params Text[] line) => _teamCityMessageWriter.WriteMessage(_lineFormatter.Format(line));

        void IStdErr.WriteLine(params Text[] errorLine) => _teamCityMessageWriter.WriteError(_lineFormatter.Format(errorLine));
    }
}