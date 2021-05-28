namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;

    internal interface IStatistics
    {
        IReadOnlyCollection<string> Errors { get; }
        
        IReadOnlyCollection<string> Warnings { get; }
        
        TimeSpan TimeElapsed { get; }

        IDisposable Start();

        void RegisterError(string error);
        
        void RegisterWarning(string warning);
    }
}