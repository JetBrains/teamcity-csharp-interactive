namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Moq;
    using Shouldly;
    using Xunit;

    public class ProcessRunnerTests
    {
        private readonly Mock<IProcessManager> _processManager = new();
        private readonly Mock<IStartInfo> _startInfo = new();
        private readonly Mock<IProcessStateProvider> _stateProvider = new();
        private readonly Mock<IProcessMonitor> _monitor = new();
        private readonly List<Output> _output = new();
        
        [Fact]
        public void ShouldKillWhenTimeoutIsExpired()
        {
            // Given
            var timeout = TimeSpan.FromSeconds(5);
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.Setup(i => i.WaitForExit(timeout)).Returns(false);
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, timeout);

            // Then
            result.ExitCode.HasValue.ShouldBeFalse();
            result.State.ShouldBe(ProcessState.Cancel);
            _processManager.Verify(i => i.WaitForExit(timeout));
            _processManager.Verify(i => i.TryKill());
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Cancel, default));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void ShouldRunWhenTimeoutIsSpecified()
        {
            // Given
            var timeout = TimeSpan.FromSeconds(5);
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.Setup(i => i.WaitForExit(timeout)).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(1);
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, timeout);

            // Then
            result.ExitCode.HasValue.ShouldBeTrue();
            result.ExitCode!.Value.ShouldBe(1);
            result.State.ShouldBe(ProcessState.Unknown);
            _processManager.Verify(i => i.WaitForExit(timeout));
            _processManager.Verify(i => i.TryKill(), Times.Never);
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, 1));
        }
        
        [Fact]
        public void ShouldRunWhenTimeoutIsNotSpecified()
        {
            // Given
            var timeout = TimeSpan.Zero;
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(1);
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, timeout);

            // Then
            result.ExitCode.HasValue.ShouldBeTrue();
            result.ExitCode!.Value.ShouldBe(1);
            result.State.ShouldBe(ProcessState.Unknown);
            _processManager.Verify(i => i.WaitForExit());
            _processManager.Verify(i => i.TryKill(), Times.Never);
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, 1));
        }
        
        [Fact]
        public void ShouldNotWaitWhenCannotStart()
        {
            // Given
            var timeout = TimeSpan.Zero;
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(false);
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, timeout);

            // Then
            result.ExitCode.HasValue.ShouldBeFalse();
            result.State.ShouldBe(ProcessState.Fail);
            _processManager.Verify(i => i.WaitForExit(), Times.Never);
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Fail, default));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, It.IsAny<int>()), Times.Never);
        }
        
        [Fact]
        public void ShouldProvideOutput()
        {
            // Given
            var timeout = TimeSpan.Zero;
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(1);
            _processManager.SetupAdd(i => i.OnOutput += Handler).Callback<Action<Output>>(i => i(new Output(_startInfo.Object, false, "out")));
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, timeout);

            // Then
            _output.Count.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldRun()
        {
            // Given
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.Setup(i => i.WaitForExit(TimeSpan.FromDays(1))).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(1);
            _processManager.SetupGet(i => i.Id).Returns(99);
            _stateProvider.Setup(i => i.GetState(1)).Returns(ProcessState.Success);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, TimeSpan.FromDays(1));

            // Then
            result.ExitCode.HasValue.ShouldBeTrue();
            result.ExitCode!.Value.ShouldBe(1);
            result.State.ShouldBe(ProcessState.Success);
            _processManager.Verify(i => i.TryKill(), Times.Never);
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Success, 1));
        }
        
        [Fact]
        public void ShouldRunWhenStateProviderIsNotDefined()
        {
            // Given
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.Setup(i => i.WaitForExit(TimeSpan.FromDays(1))).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(1);
            _processManager.SetupGet(i => i.Id).Returns(99);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_startInfo.Object, Handler, default, _monitor.Object, TimeSpan.FromDays(1));

            // Then
            result.ExitCode.HasValue.ShouldBeTrue();
            result.ExitCode!.Value.ShouldBe(1);
            result.State.ShouldBe(ProcessState.Unknown);
            _processManager.Verify(i => i.TryKill(), Times.Never);
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, 1));
        }
        
        [Fact]
        public async Task ShouldRunAsync()
        {
            // Given
            _processManager.Setup(i => i.Start(_startInfo.Object)).Returns(true);
            _processManager.SetupGet(i => i.ExitCode).Returns(2);
            _processManager.SetupAdd(i => i.OnExit += It.IsAny<Action>()).Callback<Action>(i => i());
            _processManager.SetupGet(i => i.Id).Returns(99);
            _stateProvider.Setup(i => i.GetState(2)).Returns(ProcessState.Unknown);
            var cancellationTokenSource = new CancellationTokenSource();
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = await instance.RunAsync(_startInfo.Object, Handler, _stateProvider.Object, _monitor.Object, cancellationTokenSource.Token);

            // Then
            result.ExitCode.HasValue.ShouldBeTrue();
            result.ExitCode!.Value.ShouldBe(2);
            result.State.ShouldBe(ProcessState.Unknown);
            _processManager.Verify(i => i.TryKill(), Times.Never);
            _monitor.Verify(i => i.Started(_startInfo.Object, 99));
            _monitor.Verify(i => i.Finished(It.IsAny<long>(), ProcessState.Unknown, 2));
        }

        private void Handler(Output output) => _output.Add(output);

        private ProcessRunner CreateInstance(CancellationTokenSource cancellationTokenSource) =>
            new(() => _processManager.Object, cancellationTokenSource);
    }
}