namespace TeamCity.CSharpInteractive.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages;
using JetBrains.TeamCity.ServiceMessages.Write;
using Moq;
using Shouldly;
using Xunit;

public class ReliableBuildResultTests
{
    private readonly Mock<ITeamCitySettings> _teamCitySettings = new();
    private readonly Mock<IFileSystem> _fileSystem = new();
    private readonly Mock<IMessagesReader> _messagesReader = new();
    private readonly Mock<IBuildResult> _baseBuildResult = new();
    private readonly Mock<IStartInfo> _startInfo = new();
    
    [Fact]
    public void ShouldProcessMessageWithoutSourceByBaseImplementation()
    {
        // Given
        var messages = Mock.Of<IReadOnlyList<BuildMessage>>();
        var result = CreateInstance();
        var message = new ServiceMessage("some message");
        _baseBuildResult.Setup(i => i.ProcessMessage(_startInfo.Object, message)).Returns(messages);

        // When
        result.ProcessMessage(_startInfo.Object, message).ShouldBe(messages);

        // Then
    }

    [Fact]
    public void ShouldProcessMessageWithSource()
    {
        // Given
        var result = CreateInstance();
        var buildResult = new BuildResult(BuildState.Succeeded).AddCommandLines(new CommandLineResult(_startInfo.Object, 33));
        var messages = Mock.Of<IReadOnlyList<BuildMessage>>();
        _baseBuildResult.Setup(i => i.ProcessMessage(_startInfo.Object, It.IsAny<IServiceMessage>())).Returns(messages);
        _baseBuildResult.Setup(i => i.Create(_startInfo.Object, ProcessState.Succeeded, 33)).Returns(buildResult);
        _teamCitySettings.SetupGet(i => i.ServiceMessagesPath).Returns("Messages");
        
        var message1 = new ServiceMessage("some message") { { "source", "Abc" }};
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Abc"))).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Abc.msg"))).Returns(true);
        var msg1 = new ServiceMessage("1");
        var msg11 = new ServiceMessage("11");
        _messagesReader.Setup(i => i.Read(Path.Combine("Messages", "Abc"), Path.Combine("Messages", "Abc.msg"))).Returns(new [] { msg1, msg11 });

        var message2 = new ServiceMessage("some message") { { "source", "Xyz" }};
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Xyz"))).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Xyz.msg"))).Returns(true);
        var msg2 = new ServiceMessage("2");
        _messagesReader.Setup(i => i.Read(Path.Combine("Messages", "Xyz"), Path.Combine("Messages", "Xyz.msg"))).Returns(new [] { msg2 });
        
        var message3 = new ServiceMessage("some message") { { "source", "Fff" }};
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Fff"))).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Fff.msg"))).Returns(true);
        
        var message4 = new ServiceMessage("some message") { { "source", "Bbb" }};
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Bbb"))).Returns(true);
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Bbb.msg"))).Returns(false);
        
        var message5 = new ServiceMessage("some message") { { "source", "Ccc" }};
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Ccc"))).Returns(false);
        _fileSystem.Setup(i => i.IsFileExist(Path.Combine("Messages", "Ccc.msg"))).Returns(false);

        // When
        result.ProcessMessage(_startInfo.Object, message3).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, message1).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, message4).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, message2).ShouldBeEmpty();
        result.ProcessMessage(_startInfo.Object, message5).ShouldBeEmpty();
        var actualBuildResult = result.Create(_startInfo.Object, ProcessState.Succeeded, 33);
        
        // Then
        actualBuildResult.ShouldBe(buildResult);
        _baseBuildResult.Verify(i => i.Create(_startInfo.Object, ProcessState.Succeeded, 33));
        _baseBuildResult.Verify(i => i.ProcessMessage(_startInfo.Object, msg1));
        _baseBuildResult.Verify(i => i.ProcessMessage(_startInfo.Object, msg11));
        _baseBuildResult.Verify(i => i.ProcessMessage(_startInfo.Object, msg2));
        _messagesReader.Verify(i => i.Read(Path.Combine("Messages", "Fff"), Path.Combine("Messages", "Fff.msg")), Times.Never);
        _messagesReader.Verify(i => i.Read(Path.Combine("Messages", "Bbb"), Path.Combine("Messages", "Bbb.msg")), Times.Never);
        _messagesReader.Verify(i => i.Read(Path.Combine("Messages", "Ccc"), Path.Combine("Messages", "Ccc.msg")), Times.Never);
    }

    private ReliableBuildResult CreateInstance() =>
        new(_teamCitySettings.Object, _fileSystem.Object, _messagesReader.Object, _baseBuildResult.Object);
}