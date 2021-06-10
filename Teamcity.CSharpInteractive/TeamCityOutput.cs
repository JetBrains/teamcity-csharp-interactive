// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using JetBrains.TeamCity.ServiceMessages.Write.Special;

    internal class TeamCityOutput : IStdOut, IStdErr, IFlushable
    {
        private readonly ITeamCityLineAcc _stdOutAcc;
        private readonly ITeamCityLineAcc _stdErrAcc;
        private readonly ITeamCityMessageWriter _teamCityMessageWriter;

        public TeamCityOutput(
            IFlushableRegistry flushableRegistry,
            ITeamCityLineAcc stdOutAcc,
            ITeamCityLineAcc stdErrAcc,
            ITeamCityMessageWriter teamCityMessageWriter)
        {
            _stdOutAcc = stdOutAcc;
            _stdErrAcc = stdErrAcc;
            _teamCityMessageWriter = teamCityMessageWriter;
            flushableRegistry.Register(this);
        }

        void IStdOut.Write(params Text[] text)
        {
            _stdOutAcc.Write(text);
            TryWriteLines(_stdOutAcc, false, false);
        }

        void IStdErr.Write(params Text[] error)
        {
            _stdErrAcc.Write(error);
            TryWriteLines(_stdErrAcc, false, true);
        }

        public void Flush()
        {
            TryWriteLines(_stdOutAcc, true, false);
            TryWriteLines(_stdErrAcc, true, true);
        }

        private void TryWriteLines(ITeamCityLineAcc acc, bool force, bool error)
        {
            foreach (var line in acc.GetLines(force))
            {
                if (error)
                {
                    _teamCityMessageWriter.WriteError(line);
                }
                else
                {
                    _teamCityMessageWriter.WriteMessage(line);
                }
            }
        }
    }
}