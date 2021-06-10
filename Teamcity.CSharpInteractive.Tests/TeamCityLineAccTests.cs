namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class TeamCityLineAccTests
    {
        private readonly Mock<IColorTheme> _colorTheme;

        public TeamCityLineAccTests()
        {
            _colorTheme = new Mock<IColorTheme>();
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Default)).Returns("D");
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Success)).Returns("S");
            _colorTheme.Setup(i => i.GetAnsiColor(Color.Error)).Returns("E");
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldAccumulateLines(Text[][] writes, bool force, string[] expectedLines, string[] expectedLastLines)
        {
            // Give
            var acc = CreateInstance();
            
            // When
            foreach (var text in writes)
            {
                acc.Write(text);
            }
            
            var actualLines = acc.GetLines(force).ToArray();
            var actualLastLines = acc.GetLines(true).ToArray();

            // Then
            actualLines.ShouldBe(expectedLines);
            actualLastLines.ShouldBe(expectedLastLines);
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), Text.NewLine},
                    new [] {new Text("Xyz", Color.Success), Text.NewLine}
                },
                false,
                new []{"Abc", "^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), Text.NewLine},
                    new [] {new Text("Xyz", Color.Success)}
                },
                true,
                new []{"Abc", "^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), Text.NewLine, new Text("Xyz", Color.Success), Text.NewLine}
                },
                false,
                new []{"Abc", "^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), Text.NewLine, Text.NewLine, new Text("Xyz", Color.Success), Text.NewLine}
                },
                false,
                new []{"Abc", "", "^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] { Text.NewLine, Text.NewLine}
                },
                false,
                new []{"", ""},
                Array.Empty<string>()
            },

            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), new Text("Xyz", Color.Success), Text.NewLine}
                },
                false,
                new []{"Abc^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), new Text("Xyz", Color.Success), new Text("PP", Color.Success), Text.NewLine}
                },
                false,
                new []{"Abc^[SmXyzPP"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), new Text("Xyz", Color.Success), new Text("pp", Color.Error), Text.NewLine}
                },
                false,
                new []{"Abc^[SmXyz^[Empp"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc", Color.Error), new Text("Xyz", Color.Success), Text.NewLine}
                },
                false,
                new []{"^[EmAbc^[SmXyz"},
                Array.Empty<string>()
            },
            
            new object[]
            {
                new[]
                {
                    new [] {new Text("Abc"), new Text("Xyz", Color.Success)}
                },
                false,
                Array.Empty<string>(),
                new []{"Abc^[SmXyz"},
            }
        };

        private TeamCityLineAcc CreateInstance() =>
            new(_colorTheme.Object) {EscapeSymbol = '^'};
    }
}