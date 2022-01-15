// ReSharper disable CheckNamespace
namespace Dotnet;

using System.Collections.Generic;

internal class EmptyStatisticsCalculator: IStatisticsCalculator
{
    public static readonly IStatisticsCalculator Shared = new EmptyStatisticsCalculator();

    private EmptyStatisticsCalculator() { }

    public BuildStatistics Calculate(IReadOnlyCollection<BuildResult> results) => BuildStatistics.Empty;
}