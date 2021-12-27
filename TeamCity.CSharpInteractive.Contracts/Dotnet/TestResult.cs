// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using Cmd;

    public readonly record struct TestResult(
        TestState State,
        string AssemblyName,
        string FullyQualifiedName,
        string DisplayName,
        string Message,
        string Details,
        TimeSpan Duration,
        IEnumerable<Output> Output);
}