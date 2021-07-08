namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using Host;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CSharpScriptCommandRunnerTests
    {
        private readonly Mock<ICSharpScriptRunner> _csharpScriptRunner;
        private readonly Mock<IObservable<SessionContent>> _sessionsSource;
        private readonly Mock<IDisposable> _subscriptionToken;

        public CSharpScriptCommandRunnerTests()
        {
            _csharpScriptRunner = new Mock<ICSharpScriptRunner>();
            _subscriptionToken = new Mock<IDisposable>();
            _sessionsSource = new Mock<IObservable<SessionContent>>();
            _sessionsSource.Setup(i => i.Subscribe(It.IsAny<IObserver<SessionContent>>())).Returns(_subscriptionToken.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldRunScriptCommand(bool result)
        {
            // Given
            var command = new ScriptCommand("abc", "code");
            var commandRunner = CreateInstance();
            _csharpScriptRunner.Setup(i => i.Run("code")).Returns(result);

            // When
            var actualResult = commandRunner.TryRun(command);

            // Then
            _csharpScriptRunner.Verify(i => i.Run("code"));
            _subscriptionToken.Verify(i => i.Dispose());
            actualResult.Command.ShouldBe(command);
            actualResult.Success.ShouldBe(result);
        }
        
        [Fact]
        public void ShouldRunResetCommand()
        {
            // Given
            var command = ResetCommand.Shared;
            var commandRunner = CreateInstance();

            // When
            var actualResult = commandRunner.TryRun(command);

            // Then
            _csharpScriptRunner.Verify(i => i.Reset());
            actualResult.Command.ShouldBe(command);
            actualResult.Success.ShouldBe(true);
        }
        
        [Fact]
        public void ShouldSkipUnhandledCommand()
        {
            // Given
            var command = new CodeCommand();
            var commandRunner = CreateInstance();

            // When
            var actualResult = commandRunner.TryRun(command);

            // Then
            actualResult.Command.ShouldBe(command);
            actualResult.Success.ShouldBe(default);
        }

        private CSharpScriptCommandRunner CreateInstance() =>
            new(Mock.Of<ILog<CSharpScriptCommandRunner>>(), _csharpScriptRunner.Object, _sessionsSource.Object);
    }
}