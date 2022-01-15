// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Dotnet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cmd;

public static partial class BuildResultExtensions
{
    public static async Task<BuildResult> WhenAll(this IEnumerable<Task<BuildResult>> results) => 
        await Task.WhenAll(results);

    public static Task<BuildResult> ContinueWith(
        this Task<BuildResult> result,
        Task<BuildResult> nextResult,
        Func<BuildResult, bool>? when = default) => 
        result.ContinueWith(currentResult => GetResult(nextResult.Result, when, currentResult.Result));
    
    public static Task<BuildResult> ContinueWith(
        this Task<BuildResult[]> results,
        Task<BuildResult> nextResult,
        Func<BuildResult, bool>? when = default) => 
        results.ContinueWith(currentResults => GetResult(nextResult.Result, when, currentResults.Result));
    
    public static Task<BuildResult> ContinueWith(
        this Task<BuildResult> result,
        Task<BuildResult[]> nextResults,
        Func<BuildResult, bool>? when = default) =>
        result.ContinueWith(currentBuild => GetResult(nextResults.Result, when, currentBuild.Result));

    public static Task<BuildResult> ContinueWith(
        this Task<BuildResult[]> results,
        Task<BuildResult[]> nextResult,
        Func<BuildResult, bool>? when = default) =>
        results.ContinueWith(currentBuilds => GetResult(nextResult.Result, when, currentBuilds.Result));
    
    private static BuildResult GetResult(BuildResult nextResult, Func<BuildResult, bool>? when, params BuildResult[] results) =>
        results.All(build => when?.Invoke(build) ?? build.State == BuildState.Succeeded)
            ? nextResult.WithResults(GetFlatResults(results.Concat(nextResult.Results)).ToArray()) : results;

    private static IEnumerable<BuildResult> GetFlatResults(IEnumerable<BuildResult> results) =>
        results
            .SelectMany(i => GetFlatResults(i.Results).Concat(Enumerable.Repeat(i, 1)))
            .Select<BuildResult, BuildResult>(i => i.WithResults(Array.Empty<BuildResult>()))
            .Distinct();
}