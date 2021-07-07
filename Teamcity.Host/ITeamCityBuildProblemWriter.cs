namespace Teamcity.Host
{
    public interface ITeamCityBuildProblemWriter
    {
        void WriteBuildProblem(string identity, string description);
    }
}