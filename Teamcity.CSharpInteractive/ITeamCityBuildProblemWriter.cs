namespace Teamcity.CSharpInteractive
{
    public interface ITeamCityBuildProblemWriter
    {
        void WriteBuildProblem(string identity, string description);
    }
}