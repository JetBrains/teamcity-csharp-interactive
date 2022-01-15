// ReSharper disable CheckNamespace
namespace Dotnet;

using System.Collections.Generic;

public interface IStatisticsCalculator
{
    BuildStatistics Calculate(IReadOnlyCollection<BuildResult> results);
}