// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class Statistics : IStatistics
    {
        private readonly Stopwatch _stopwatch = new();
        private readonly List<string> _errors = new();
        private readonly List<string> _warnings = new();

        public IReadOnlyCollection<string> Errors => _errors;

        public IReadOnlyCollection<string> Warnings => _warnings;

        public TimeSpan TimeElapsed => _stopwatch.Elapsed;

        public IDisposable Start()
        {
            _stopwatch.Start();
            return Disposable.Create(() => _stopwatch.Stop());
        }

        public void RegisterError(string error) => _errors.Add(error);

        public void RegisterWarning(string warning) => _warnings.Add(warning);
    }
}