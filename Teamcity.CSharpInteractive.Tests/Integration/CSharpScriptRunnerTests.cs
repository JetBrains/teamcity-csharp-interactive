namespace Teamcity.CSharpInteractive.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Scripting;
    using Moq;
    using Shouldly;
    using Xunit;

    public class CSharpScriptRunnerTests
    {
        private readonly Mock<ILog<CSharpScriptRunner>> _log;
        private readonly Mock<IPresenter<ScriptState<object>>> _scriptStatePresenter;
        private readonly Mock<IPresenter<CompilationDiagnostics>> _diagnosticsPresenter;
        private readonly Mock<IHost> _host;
        private readonly List<Text> _errors = new();
        private readonly List<Diagnostic> _diagnostics = new();

        public CSharpScriptRunnerTests()
        {
            _log = new Mock<ILog<CSharpScriptRunner>>();
            _log.Setup(i => i.Error(It.IsAny<ErrorId>(), It.IsAny<Text[]>())).Callback<ErrorId, Text[]>((_, i) => _errors.AddRange(i));
            _scriptStatePresenter = new Mock<IPresenter<ScriptState<object>>>();
            _diagnosticsPresenter = new Mock<IPresenter<CompilationDiagnostics>>();
            _diagnosticsPresenter.Setup(i => i.Show(It.IsAny<CompilationDiagnostics>())).Callback<CompilationDiagnostics>(i => _diagnostics.AddRange(i.Diagnostics));
            _host = new Mock<IHost>();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldRunValidCode(string script)
        {
            // Given
            var runner = CreateInstance();
            
            // When
            var result = runner.Run(Mock.Of<ICommand>(), script);

            // Then
            result.ShouldBeTrue();
            _errors.Count.ShouldBe(0);
            _diagnostics.Count.ShouldBe(0);
            _diagnosticsPresenter.Verify(i => i.Show(It.IsAny<CompilationDiagnostics>()));
            _scriptStatePresenter.Verify(i => i.Show(It.IsAny<ScriptState<object>>()));
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[] {"int i=10;"},
            // Multiline
            new object[] {"class Abc" + Environment.NewLine + "{}"},
            // using System;
            new object[] {"Console.WriteLine(10);"}
    };
        
        [Fact]
        public void ShouldPreserveState()
        {
            // Given
            var runner = CreateInstance();
            
            // When
            runner.Run(Mock.Of<ICommand>(), "int i=10;");
            var result = runner.Run(Mock.Of<ICommand>(), "Console.WriteLine(i);");

            // Then
            result.ShouldBeTrue();
            _errors.Count.ShouldBe(0);
            _diagnostics.Count.ShouldBe(0);
            _diagnosticsPresenter.Verify(i => i.Show(It.IsAny<CompilationDiagnostics>()));
            _scriptStatePresenter.Verify(i => i.Show(It.IsAny<ScriptState<object>>()));
        }
        
        [Fact]
        public void ShouldResetState()
        {
            // Given
            var runner = CreateInstance();
            
            // When
            runner.Run(Mock.Of<ICommand>(), "int i=10;");
            runner.Reset();
            var result = runner.Run(Mock.Of<ICommand>(), "Console.WriteLine(i);");

            // Then
            result.ShouldBeFalse();
            _errors.Count.ShouldBe(0);
            _diagnostics.Count.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldRunInvalidCode()
        {
            // Given
            var runner = CreateInstance();
            
            // When
            var result = runner.Run(Mock.Of<ICommand>(), "string i=10;");

            // Then
            result.ShouldBeFalse();
            _errors.Count.ShouldBe(0);
            _diagnostics.Count.ShouldNotBe(0);
            _scriptStatePresenter.Verify(i => i.Show(It.IsAny<ScriptState<object>>()), Times.Never);
        }
        
        [Fact]
        public void ShouldRunInvalidCodeAfterValid()
        {
            // Given
            var runner = CreateInstance();
            
            // When
            runner.Run(Mock.Of<ICommand>(), "int j=10;");
            var result = runner.Run(Mock.Of<ICommand>(), "string i=10;");

            // Then
            result.ShouldBeFalse();
            _errors.Count.ShouldBe(0);
            _diagnostics.Count.ShouldNotBe(0);
            _scriptStatePresenter.Verify(i => i.Show(It.IsAny<ScriptState<object>>()));
        }
        
        [Fact]
        public void ShouldRunCodeWithException()
        {
            // Given
            var runner = CreateInstance();
            
            // When
            var result = runner.Run(Mock.Of<ICommand>(), "throw new Exception();");

            // Then
            result.ShouldBeFalse();
            _errors.Count.ShouldNotBe(0);
            _diagnostics.Count.ShouldBe(0);
            _scriptStatePresenter.Verify(i => i.Show(It.IsAny<ScriptState<object>>()));
        }

        private CSharpScriptRunner CreateInstance() =>
            new(_log.Object, _scriptStatePresenter.Object, _diagnosticsPresenter.Object, _host.Object);
    }
}