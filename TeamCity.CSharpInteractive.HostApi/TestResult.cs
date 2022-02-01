// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NotAccessedPositionalProperty.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace HostApi;

using System.Diagnostics;
using System.Text;
using Immutype;

[Target]
[DebuggerTypeProxy(typeof(TestResultDebugView))]
public readonly record struct TestResult(
    TestState State,
    string Name,
    string FlowId,
    string SuiteName,
    string FullyQualifiedName,
    string DisplayName,
    string Message,
    string Details,
    TimeSpan Duration,
    IReadOnlyList<Output> Output,
    string Source,
    string CodeFilePath,
    Guid Id,
    Uri? ExecutorUri,
    int? LineNumber)
{
    public TestResult(TestState state, string name)
        : this(
            state,
            name,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            TimeSpan.Zero,
            Array.Empty<Output>(),
            string.Empty,
            string.Empty,
            Guid.Empty,
            default,
            default)
    { }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(SuiteName))
        {
            sb.Append(SuiteName);
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
        
        public string Name => _testResult.Name;

        public string SuiteName => _testResult.SuiteName;

        public string FullyQualifiedName => _testResult.FullyQualifiedName;

        public string DisplayName => _testResult.DisplayName;

        public string Message => _testResult.Message;

        public string Details => _testResult.Details;

        public TimeSpan Duration => _testResult.Duration;

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public IReadOnlyList<Output> Output => _testResult.Output;
        
        public string Source => _testResult.Source;
        
        public string CodeFilePath => _testResult.CodeFilePath;
        
        public Guid Id => _testResult.Id;
        
        public Uri? ExecutorUri => _testResult.ExecutorUri;
        
        public int? LineNumber => _testResult.LineNumber;
    }
}