namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal interface ITeamCityLineAcc
    {
        void Write(params Text[] text);

        IEnumerable<string> GetLines(bool includingIncomplete);
    }
}