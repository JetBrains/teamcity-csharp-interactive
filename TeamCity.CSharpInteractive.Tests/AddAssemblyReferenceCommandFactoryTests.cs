namespace TeamCity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class AddAssemblyReferenceCommandFactoryTests
    {
        private readonly Mock<ILog<AddAssemblyReferenceCommandFactory>> _log;
        private readonly Mock<IFilePathResolver> _assemblyPathResolver;
        private readonly List<Text> _errors = new();

        public AddAssemblyReferenceCommandFactoryTests()
        {
            _log = new Mock<ILog<AddAssemblyReferenceCommandFactory>>();
            _log.Setup(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>())).Callback<ErrorId, Text[]>((_, text) => _errors.AddRange(text));
            _assemblyPathResolver = new Mock<IFilePathResolver>();
            string fullAssemblyPath = "wd/Abc.dll";
            _assemblyPathResolver.Setup(i => i.TryResolve("Abc.dll", out fullAssemblyPath)).Returns(true);
        }
        
        [Fact]
        public void ShouldProvideOrder()
        {
            // Given
            var factory = CreateInstance();
            
            // When
            
            // Then
            factory.Order.ShouldBe(1);
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void ShouldCreateCommands(string replCommand, ICommand[] expectedCommands, bool hasErrors)
        {
            // Given
            var factory = CreateInstance();

            // When
            var actualCommands = factory.Create(replCommand).ToArray();

            // Then
            actualCommands.ShouldBe(expectedCommands);
            _errors.Any().ShouldBe(hasErrors);
        }

        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[]
            {
                "#r \"Abc.dll\"",
                new [] { new ScriptCommand("Abc.dll", "#r \"wd/Abc.dll\"") },
                false
            },
            
            new object[]
            {
                "#r   \"Abc.dll\"  ",
                new [] { new ScriptCommand("Abc.dll", "#r \"wd/Abc.dll\"") },
                false
            },

            new object[]
            {
                "#r \"Xyz.dll\"",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "#r Abc.dll",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "r \"Abc.dll\"",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "\"Abc.dll\"",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "Abc.dll",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "   ",
                Array.Empty<ICommand>(),
                false
            },
            
            new object[]
            {
                "",
                Array.Empty<ICommand>(),
                false
            }
        };

        private AddAssemblyReferenceCommandFactory CreateInstance() =>
            new(_log.Object, _assemblyPathResolver.Object);
    }
}