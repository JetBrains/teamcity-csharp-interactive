// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using JetBrains.TeamCity.ServiceMessages.Write.Special;

internal class TeamCityInOut : IStdOut, IStdErr
{
    private readonly ITeamCityLineFormatter _lineFormatter;
    private readonly ITeamCityWriter _teamCityWriter;

    public TeamCityInOut(
        ITeamCityLineFormatter lineFormatter,
        ITeamCityWriter teamCityWriter)
    {
        _lineFormatter = lineFormatter;
        _teamCityWriter = teamCityWriter;
    }

    public void Write(params Text[] text) => _teamCityWriter.WriteMessage(_lineFormatter.Format(text));

    void IStdOut.WriteLine(params Text[] line) => _teamCityWriter.WriteMessage(_lineFormatter.Format(line));

    void IStdErr.WriteLine(params Text[] errorLine) => _teamCityWriter.WriteError(_lineFormatter.Format(errorLine));
}