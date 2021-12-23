// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Cmd;
    using Dotnet;
    using JetBrains.TeamCity.ServiceMessages;

    internal class BuildResult : IBuildResult
    {
        private readonly ITestDisplayNameToFullyQualifiedNameConverter _testDisplayNameToFullyQualifiedNameConverter;
        private readonly List<BuildMessage> _messages = new();
        private readonly List<TestResult> _tests = new();
        private readonly Dictionary<TestKey, TestContext> _currentTests = new();

        public BuildResult(
            ITestDisplayNameToFullyQualifiedNameConverter testDisplayNameToFullyQualifiedNameConverter) =>
            _testDisplayNameToFullyQualifiedNameConverter = testDisplayNameToFullyQualifiedNameConverter;

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public IEnumerable<BuildMessage> ProcessOutput(IStartInfo startInfo, IReadOnlyList<IServiceMessage> messages)
        {
            return messages
                .Select(
                    message => message.Name.ToLowerInvariant() switch
                    {
                        "teststdout" => OnStdOut(message, startInfo),
                        "teststderr" => OnStdErr(message, startInfo),
                        "testfinished" => OnTestFinished(message),
                        "testignored " => OnTestIgnored(message),
                        "testfailed " => OnTestFailed(message),
                        "message" => OnMessage(message),
                        "buildproblem" => OnBuildProblem(message),
                        _ => new[] { new BuildMessage(BuildMessageState.ServiceMessage, messages) }
                    })
                .SelectMany(i => i)
                .ToArray();
        }

        public Dotnet.BuildResult CreateResult(int? exitCode) =>
            new(exitCode, _messages.AsReadOnly(), _tests.AsReadOnly());

        private IEnumerable<BuildMessage> OnStdOut(IServiceMessage message, IStartInfo startInfo)
        {
            var output = message.GetValue("out") ?? string.Empty;
            GetTestContext(message).AddStdOut(startInfo, output);
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
            yield return new BuildMessage(BuildMessageState.Info, Array.Empty<IServiceMessage>(), output);
        }

        private IEnumerable<BuildMessage> OnStdErr(IServiceMessage message, IStartInfo info)
        {
            var output = message.GetValue("out") ?? string.Empty;
            GetTestContext(message).AddStdErr(info, output);
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
            yield return new BuildMessage(BuildMessageState.Error, Array.Empty<IServiceMessage>(), output);
        }

        private IEnumerable<BuildMessage> OnTestFinished(IServiceMessage message)
        {
            var ctx = GetTestContext(message, true);
            var durationStrMs = message.GetValue("duration");
            var duration = TimeSpan.Zero;
            if (!string.IsNullOrWhiteSpace(durationStrMs) && int.TryParse(durationStrMs, out var durationMs))
            {
                duration = TimeSpan.FromMilliseconds(durationMs);
            }

            _tests.Add(
                new TestResult(
                    TestState.Finished,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    string.Empty,
                    string.Empty,
                    duration,
                    ctx.Output));
            
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
        }

        private IEnumerable<BuildMessage> OnTestIgnored(IServiceMessage message)
        {
            var ctx = GetTestContext(message, true);
            _tests.Add(
                new TestResult(
                    TestState.Ignored,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    message.GetValue("message") ?? string.Empty,
                    string.Empty,
                    TimeSpan.Zero,
                    ctx.Output));
            
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
        }

        private IEnumerable<BuildMessage> OnTestFailed(IServiceMessage message)
        {
            var ctx = GetTestContext(message, true);
            _tests.Add(
                new TestResult(
                    TestState.Failed,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    message.GetValue("message") ?? string.Empty,
                    message.GetValue("details") ?? string.Empty,
                    TimeSpan.Zero,
                    ctx.Output));
            
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
        }

        private IEnumerable<BuildMessage> OnMessage(IServiceMessage message)
        {
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
            var text = message.GetValue("text") ?? string.Empty;
            var statusStr = message.GetValue("status");
            if (Enum.TryParse<BuildMessageState>(statusStr, false, out var status))
            {
                var errorDetails = message.GetValue("errorDetails") ?? string.Empty;
                var buildMessage = new BuildMessage(status, Array.Empty<IServiceMessage>(), text, errorDetails);
                _messages.Add(buildMessage);
                yield return buildMessage;
            }
            else
            {
                yield return new BuildMessage(BuildMessageState.Info, Array.Empty<IServiceMessage>(), text);
            }
        }

        private IEnumerable<BuildMessage> OnBuildProblem(IServiceMessage message)
        {
            yield return new BuildMessage(BuildMessageState.ServiceMessage, new List<IServiceMessage>(1) { message }.AsReadOnly());
            var description = message.GetValue("description") ?? string.Empty;
            var identity = message.GetValue("identity") ?? string.Empty;
            var buildMessage = new BuildMessage(BuildMessageState.BuildProblem, Array.Empty<IServiceMessage>(), description, identity);
            _messages.Add(buildMessage);
            yield return buildMessage;
        }
        
        private TestContext GetTestContext(IServiceMessage message, bool remove = false)
        {
            var testKey = new TestKey(message);
            if (!_currentTests.TryGetValue(testKey, out var testContext))
            {
                testContext = new TestContext(testKey.TestName);
                if (!remove)
                {
                    _currentTests.Add(testKey, testContext);
                }
            }
            else
            {
                if (remove)
                {
                    _currentTests.Remove(testKey);
                }
            }

            return testContext;
        }
        
        // ReSharper disable once NotAccessedPositionalProperty.Local
        private readonly record struct TestKey(string FlowId, string TestName)
        {
            public TestKey(IServiceMessage message) :
                this(message.GetValue("flowId") ?? string.Empty, message.GetValue("name") ?? string.Empty)
            { }
        }
    }
}