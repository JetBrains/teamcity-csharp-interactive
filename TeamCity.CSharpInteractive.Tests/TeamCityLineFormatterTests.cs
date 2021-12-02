namespace TeamCity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Contracts;
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCityLineFormatterTests
    {
        private readonly Mock<IColorTheme> _colorTheme;

        public TeamCityLineFormatterTests()
        {
            _colorTheme = new Mock<IColorTheme>();
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Default)).Returns("D");
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Success)).Returns("S");
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Error)).Returns("E");
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldFormat(Text[] line, string expectedLine)
        {
            // Give
            var formatter = CreateInstance();
            
            // When
            var actualLine = formatter.Format(line);

            // Then
            actualLine.ShouldBe(expectedLine);
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[] {new [] {new Text("Abc")}, "Abc"},
            new object[] {new [] {new Text("Abc", Color.Error)}, "^[EmAbc"},
            new object[] {new [] {new Text("Abc", Color.Error), new Text("Xyz", Color.Error)}, "^[EmAbcXyz"},
            new object[] {new [] {new Text("1"), new Text("Abc", Color.Error), new Text("Xyz", Color.Error), new Text("2")}, "1^[EmAbcXyz^[Dm2"},
            new object[] {new [] {new Text("", Color.Error)}, ""},
            new object[] {new [] {new Text("   ", Color.Error)}, "   "},
        };

        private TeamCityLineFormatter CreateInstance() =>
            new(_colorTheme.Object) {EscapeSymbol = '^'};
    }
}