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
        private readonly IStatisticsCalculator _statisticsCalculator;
        private readonly ITestDisplayNameToFullyQualifiedNameConverter _testDisplayNameToFullyQualifiedNameConverter;
        private readonly List<BuildMessage> _errors = new();
        private readonly List<BuildMessage> _warnings = new();
        private readonly List<TestResult> _tests = new();
        private readonly HashSet<TestKey> _testKeys = new();
        private readonly Dictionary<TestKey, TestContext> _currentTests = new();
        private readonly Dictionary<string, LinkedList<string>> _assemblies = new();

        public BuildResult(
            IStatisticsCalculator statisticsCalculator,
            ITestDisplayNameToFullyQualifiedNameConverter testDisplayNameToFullyQualifiedNameConverter)
        {
            _statisticsCalculator = statisticsCalculator;
            _testDisplayNameToFullyQualifiedNameConverter = testDisplayNameToFullyQualifiedNameConverter;
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public IReadOnlyList<BuildMessage> ProcessMessage(IStartInfo startInfo, int processId, IServiceMessage message)
        {
            return message.Name.ToLowerInvariant() switch
            {
                "teststdout" => OnStdOut(message, startInfo, processId).ToArray(),
                "teststderr" => OnStdErr(message, startInfo, processId).ToArray(),
                "testsuitestarted" => OnTestSuiteStarted(message).ToArray(),
                "testsuitefinished" => OnTestSuiteFinished(message).ToArray(),
                "testfinished" => OnTestFinished(message).ToArray(),
                "testignored" => OnTestIgnored(message).ToArray(),
                "testfailed" => OnTestFailed(message).ToArray(),
                "message" => OnMessage(message).ToArray(),
                "buildproblem" => OnBuildProblem(message).ToArray(),
                _ => new[]
                {
                    new BuildMessage(BuildMessageState.ServiceMessage, message)
                }
            };
        }

        public Dotnet.BuildResult Create(IStartInfo startInfo, ProcessState state, int? exitCode)
        {
            var buildState = state switch
            {
                ProcessState.Failed => BuildState.Failed,
                ProcessState.Canceled => BuildState.Canceled,
                ProcessState.Succeeded => BuildState.Succeeded,
                _ => BuildState.Succeeded
            };

            return new Dotnet.BuildResult(
                buildState,
                _statisticsCalculator,
                _errors.AsReadOnly(),
                _warnings.AsReadOnly(),
                _tests.AsReadOnly(),
                new []{ new CommandLineResult(startInfo, exitCode) },
                Array.Empty<Dotnet.BuildResult>());
        }

        private IEnumerable<BuildMessage> OnStdOut(IServiceMessage message, IStartInfo startInfo, int processId)
        {
            var testKey = CreateKey(message);
            var output = message.GetValue("out") ?? string.Empty;
            GetTestContext(testKey).AddStdOut(startInfo, processId, output);
            yield return new BuildMessage(BuildMessageState.Info).WithText(output);
        }

        private IEnumerable<BuildMessage> OnStdErr(IServiceMessage message, IStartInfo info, int processId)
        {
            var testKey = CreateKey(message);
            var output = message.GetValue("out") ?? string.Empty;
            GetTestContext(testKey).AddStdErr(info, processId, output);
            yield return new BuildMessage(BuildMessageState.Error).WithText(output);
        }

        private IEnumerable<BuildMessage> OnTestSuiteStarted(IServiceMessage message)
        {
            var name = message.GetValue("name") ?? string.Empty;
            var flowId = message.GetValue("flowId") ?? string.Empty;
            if(!_assemblies.TryGetValue(flowId, out var names))
            {
                names = new LinkedList<string>();
                _assemblies.Add(flowId, names);
            }
            
            names.AddLast(name);
            yield break;
        }
        
        private IEnumerable<BuildMessage> OnTestSuiteFinished(IServiceMessage message)
        {
            var name = message.GetValue("name") ?? string.Empty;
            var flowId = message.GetValue("flowId") ?? string.Empty;
            if(_assemblies.TryGetValue(flowId, out var names) && name.Length > 0)
            {
                names.RemoveLast();
            }
            
            yield break;
        }

        private IEnumerable<BuildMessage> OnTestFinished(IServiceMessage message)
        {
            var testKey = CreateKey(message);
            if (_testKeys.Remove(testKey))
            {
                yield break;
            }

            var ctx = GetTestContext(testKey, true);
            var durationStrMs = message.GetValue("duration");
            var duration = TimeSpan.Zero;
            if (!string.IsNullOrWhiteSpace(durationStrMs) && int.TryParse(durationStrMs, out var durationMs))
            {
                duration = TimeSpan.FromMilliseconds(durationMs);
            }

            _tests.Add(
                new TestResult(
                    TestState.Passed,
                    testKey.AssemblyName,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    string.Empty,
                    string.Empty,
                    duration,
                    ctx.Output));
        }

        private IEnumerable<BuildMessage> OnTestIgnored(IServiceMessage message)
        {
            var testKey = CreateKey(message);
            _testKeys.Add(testKey);
            var ctx = GetTestContext(testKey, true);
            _tests.Add(
                new TestResult(
                    TestState.Ignored,
                    testKey.AssemblyName,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    message.GetValue("message") ?? string.Empty,
                    string.Empty,
                    TimeSpan.Zero,
                    ctx.Output));
            
            yield break;
        }

        private IEnumerable<BuildMessage> OnTestFailed(IServiceMessage message)
        {
            var testKey = CreateKey(message);
            _testKeys.Add(testKey);
            var ctx = GetTestContext(testKey, true);
            _tests.Add(
                new TestResult(
                    TestState.Failed,
                    testKey.AssemblyName,
                    _testDisplayNameToFullyQualifiedNameConverter.Convert(ctx.Name),
                    ctx.Name,
                    message.GetValue("message") ?? string.Empty,
                    message.GetValue("details") ?? string.Empty,
                    TimeSpan.Zero,
                    ctx.Output));
            
            yield break;
        }

        private IEnumerable<BuildMessage> OnMessage(IServiceMessage message)
        {
            var text = message.GetValue("text") ?? string.Empty;
            var state = message.GetValue("status").ToUpperInvariant() switch
            {
                "WARNING" => BuildMessageState.Warning,
                "FAILURE" => BuildMessageState.Failure,
                "ERROR" => BuildMessageState.Error,
                _ => BuildMessageState.Info
            };

            var errorDetails = message.GetValue("errorDetails") ?? string.Empty;
            var buildMessage = new BuildMessage(state, default, text, errorDetails);
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (state)
            {
                case BuildMessageState.Warning:
                    _warnings.Add(buildMessage);
                    break;

                case BuildMessageState.Failure:
                case BuildMessageState.Error:
                // ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
                case BuildMessageState.BuildProblem:
                    _errors.Add(buildMessage);
                    break;
            }

            yield return buildMessage;
        }

        private IEnumerable<BuildMessage> OnBuildProblem(IServiceMessage message)
        {
            var description = message.GetValue("description") ?? string.Empty;
            var identity = message.GetValue("identity") ?? string.Empty;
            var buildMessage = new BuildMessage(BuildMessageState.BuildProblem, default, description, identity);
            _errors.Add(buildMessage);
            yield return buildMessage;
        }
        
        private TestContext GetTestContext(TestKey testKey, bool remove = false)
        {
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

        private string GetAssemblyName(string flowId)
        {
            if (!_assemblies.TryGetValue(flowId, out var names) || names.Count == 0)
            {
                return string.Empty;
            }

            return string.Join('.', names);
        }

        private TestKey CreateKey(IServiceMessage message)
        {
            var flowId = message.GetValue("flowId") ?? string.Empty;
            var assemblyName = GetAssemblyName(flowId);
            var name = message.GetValue("name") ?? string.Empty;
            return new TestKey(flowId, assemblyName, name);
        }
        
        // ReSharper disable once NotAccessedPositionalProperty.Local
        private readonly record struct TestKey(string FlowId, string AssemblyName, string TestName);
        
        private class TestContext
        {
            public readonly string Name;
            public readonly List<Output> Output = new();

            public TestContext(string name) => Name = name;

            public void AddStdOut(IStartInfo info, int processId, string? text)
            {
                if (text != default)
                {
                    Output.Add(new Output(info, false, text, processId));
                }
            }

            public void AddStdErr(IStartInfo info, int processId, string? error)
            {
                if (error != default)
                {
                    Output.Add(new Output(info, true, error, processId));
                }
            }
        }
    }
}