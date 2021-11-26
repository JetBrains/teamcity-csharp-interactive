namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Cmd;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CommandLineServiceTests
    {
        private readonly Mock<IProcess> _process;
        private readonly CommandLine _commandLine;
        private readonly List<CommandLineOutput> _output = new();

        public CommandLineServiceTests()
        {
            _process = new Mock<IProcess>();
            _commandLine = new CommandLine("cmd");
        }

        [Fact]
        public void ShouldKillWhenTimeoutIsExpired()
        {
            // Given
            var timeout = TimeSpan.FromSeconds(5);
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(true);
            _process.Setup(i => i.WaitForExit(timeout)).Returns(false);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_commandLine, Handler, timeout);

            // Then
            result.HasValue.ShouldBeFalse();
            _process.Verify(i => i.WaitForExit(timeout));
            _process.Verify(i => i.Kill());
        }
        
        [Fact]
        public void ShouldRunWhenTimeoutIsSpecified()
        {
            // Given
            var timeout = TimeSpan.FromSeconds(5);
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(true);
            _process.Setup(i => i.WaitForExit(timeout)).Returns(true);
            _process.SetupGet(i => i.ExitCode).Returns(1);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_commandLine, Handler, timeout);

            // Then
            result.HasValue.ShouldBeTrue();
            result!.Value.ShouldBe(1);
            _process.Verify(i => i.WaitForExit(timeout));
            _process.Verify(i => i.Kill(), Times.Never);
        }
        
        [Fact]
        public void ShouldRunWhenTimeoutIsNotSpecified()
        {
            // Given
            var timeout = TimeSpan.Zero;
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(true);
            _process.SetupGet(i => i.ExitCode).Returns(1);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_commandLine, Handler, timeout);

            // Then
            result.HasValue.ShouldBeTrue();
            result!.Value.ShouldBe(1);
            _process.Verify(i => i.WaitForExit());
            _process.Verify(i => i.Kill(), Times.Never);
        }
        
        [Fact]
        public void ShouldNotWaitWhenCannotStart()
        {
            // Given
            var timeout = TimeSpan.Zero;
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(false);
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = instance.Run(_commandLine, Handler, timeout);

            // Then
            result.HasValue.ShouldBeFalse();
            _process.Verify(i => i.WaitForExit(), Times.Never);
        }
        
        [Fact]
        public void ShouldProvideOutput()
        {
            // Given
            var timeout = TimeSpan.Zero;
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(true);
            _process.SetupGet(i => i.ExitCode).Returns(1);
            _process.SetupAdd(i => i.OnOutput += Handler).Callback<Action<CommandLineOutput>>(i => i(new CommandLineOutput(_commandLine, false, "out")));
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            instance.Run(_commandLine, Handler, timeout);

            // Then
            _output.Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task ShouldRunAsync()
        {
            // Given
            ProcessStartInfo startInfo = new();
            _process.Setup(i => i.Start(_commandLine, out startInfo)).Returns(true);
            _process.SetupGet(i => i.ExitCode).Returns(1);
            _process.SetupAdd(i => i.OnExit += It.IsAny<Action>()).Callback<Action>(i => i());
            var cancellationTokenSource = new CancellationTokenSource();
            var instance = CreateInstance(new CancellationTokenSource());

            // When
            var result = await instance.RunAsync(_commandLine, Handler, cancellationTokenSource.Token);

            // Then
            result.HasValue.ShouldBeTrue();
            result!.Value.ShouldBe(1);
            _process.Verify(i => i.Kill(), Times.Never);
        }
        
        private void Handler(CommandLineOutput output) => _output.Add(output);

        private CommandLineService CreateInstance(CancellationTokenSource cancellationTokenSource) =>
            new(Mock.Of<ILog<CommandLineService>>(), () => _process.Object, cancellationTokenSource);
    }
}