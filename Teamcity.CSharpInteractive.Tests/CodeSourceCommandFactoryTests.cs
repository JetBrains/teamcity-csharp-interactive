namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CodeSourceCommandFactoryTests
    {
        private const string SourceName = "Abc";
        private static readonly ScriptCommand ScriptCommand11 = new ScriptCommand(SourceName, "code11");
        private static readonly ScriptCommand ScriptCommand12 = new ScriptCommand(SourceName, "code12");
        private static readonly ScriptCommand ScriptCommand2 = new ScriptCommand(SourceName, "code2");
        private static readonly ScriptCommand ScriptCommand3 = new ScriptCommand(SourceName, "#code3");
        private readonly Mock<ILog<CodeSourceCommandFactory>> _log;
        private readonly Mock<ICommandFactory<string>> _replCommandFactory1;
        private readonly Mock<ICommandFactory<string>> _replCommandFactory2;
        private readonly Mock<ICommandFactory<ScriptCommand>> _scriptCommandFactory;
        private readonly Mock<ICodeSource> _codeSource;

        public CodeSourceCommandFactoryTests()
        {
            _log = new Mock<ILog<CodeSourceCommandFactory>>();
            _replCommandFactory1 = new Mock<ICommandFactory<string>>();
            _replCommandFactory1.Setup(i => i.Create(It.IsAny<string>())).Returns(Enumerable.Empty<ICommand>());
            _replCommandFactory2 = new Mock<ICommandFactory<string>>();
            _replCommandFactory2.Setup(i => i.Create("#help")).Returns(new [] {HelpCommand.Shared });
            _scriptCommandFactory = new Mock<ICommandFactory<ScriptCommand>>();
            _scriptCommandFactory.Setup(i => i.Create(new ScriptCommand(SourceName, "code1" + Environment.NewLine))).Returns(new[] {ScriptCommand11, ScriptCommand12});
            _scriptCommandFactory.Setup(i => i.Create(new ScriptCommand(SourceName, "code2" + Environment.NewLine))).Returns(new[] {ScriptCommand2});
            _scriptCommandFactory.Setup(i => i.Create(new ScriptCommand(SourceName, "#code3" + Environment.NewLine))).Returns(new[] {ScriptCommand3});
            _codeSource = new Mock<ICodeSource>();
        }
        
        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldCreateCodeSource(bool resetRequired, IEnumerable<string> lines, ICommand[] expectedCommands)
        {
            // Given
            _codeSource.SetupGet(i => i.Name).Returns(SourceName);
            _codeSource.SetupGet(i => i.ResetRequired).Returns(resetRequired);
            _codeSource.Setup(i => i.GetEnumerator()).Returns(lines.GetEnumerator());
            var factory = CreateInstance();

            // When
            var actualCommands = factory.Create(_codeSource.Object).ToArray();

            // Then
            actualCommands.ShouldBe(expectedCommands);
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[]
            {
                true,
                new[] {"code1", "#help", "code2"},
                new[] {ResetCommand.Shared, ScriptCommand11, ScriptCommand12, HelpCommand.Shared, ScriptCommand2}
            },
            new object[]
            {
                true,
                new[] {"#code3", "#help", "code2"},
                new[] {ResetCommand.Shared, ScriptCommand3, HelpCommand.Shared, ScriptCommand2}
            },
            new object[]
            {
                false,
                new[] {"code1", "#help", "code2"},
                new[] {ScriptCommand11, ScriptCommand12, HelpCommand.Shared, ScriptCommand2}
            }
        };

        private CodeSourceCommandFactory CreateInstance() =>
            new(
                _log.Object,
                new[] {_replCommandFactory1.Object, _replCommandFactory2.Object},
                _scriptCommandFactory.Object);
    }
}