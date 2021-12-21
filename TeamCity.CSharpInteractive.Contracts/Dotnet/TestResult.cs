// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using System;
    using System.Collections.Generic;
    using Cmd;

    public record TestResult(
        TestState State,
        string FullyQualifiedName,
        string DisplayName,
        string Message,
        string Details,
        TimeSpan Duration,
        IEnumerable<CommandLineOutput> Output);
}