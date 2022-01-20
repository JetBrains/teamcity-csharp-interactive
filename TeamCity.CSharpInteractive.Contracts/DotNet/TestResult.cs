// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable CheckNamespace
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace DotNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Cmd;

    [Immutype.Target]
    [DebuggerTypeProxy(typeof(TestResultDebugView))]
    public readonly record struct TestResult(
        TestState State,
        string AssemblyName,
        string FullyQualifiedName,
        string DisplayName,
        string Message,
        string Details,
        TimeSpan Duration,
        IReadOnlyList<Output> Output)
    {
        public TestResult(TestState state, string displayName)
            : this(state,string.Empty, string.Empty, displayName, string.Empty, string.Empty, TimeSpan.Zero, Array.Empty<Output>())
        { }
        
        public static implicit operator string(TestResult it) => it.ToString();

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(AssemblyName))
            {
                sb.Append(AssemblyName);
                sb.Append(": ");
            }

            sb.Append(DisplayName);
            sb.Append(" is ");
            sb.Append(State.ToString().ToLowerInvariant());
            sb.Append('.');
            return sb.ToString();
        }

        private class TestResultDebugView
        {
            private readonly TestResult _testResult;

            public TestResultDebugView(TestResult testResult) => _testResult = testResult;

            public TestState State => _testResult.State;

            public string AssemblyName => _testResult.AssemblyName;

            public string FullyQualifiedName => _testResult.FullyQualifiedName;

            public string DisplayName => _testResult.DisplayName;

            public string Message => _testResult.Message;

            public string Details => _testResult.Details;

            public TimeSpan Duration => _testResult.Duration;

            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public IReadOnlyList<Output> Output => _testResult.Output;
        }
    }
}