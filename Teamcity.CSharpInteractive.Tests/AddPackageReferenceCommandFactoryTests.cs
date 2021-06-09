namespace Teamcity.CSharpInteractive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NuGet.Versioning;
    using Shouldly;
    using Xunit;

    public class AddPackageReferenceCommandFactoryTests
    {
        private readonly Mock<ILog<AddPackageReferenceCommandFactory>> _log;
        private readonly List<Text> _errors = new();

        public AddPackageReferenceCommandFactoryTests()
        {
            _log = new Mock<ILog<AddPackageReferenceCommandFactory>>();
            _log.Setup(i => i.Error(It.IsAny<Text[]>())).Callback<Text[]>(text => _errors.AddRange(text));
        }

        [Theory]
        [MemberData(nameof(Data))]
        internal void Should(string replCommand, ICommand[] expectedCommands, bool hasErrors)
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
                Array.Empty<ICommand>(),
                false
            },
            new object[]
            {
                "#",
                Array.Empty<ICommand>(),
                false
            },
            new object[]
            {
                "#   ",
                Array.Empty<ICommand>(),
                false
            },
            new object[]
            {
                "",
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
                "#r Abc 1.2.3",
                new [] {new AddPackageReferenceCommand("Abc", new NuGetVersion(1,2,3))},
                false
            },
            new object[]
            {
                "  #r  Abc    1.2.3 ",
                new [] {new AddPackageReferenceCommand("Abc", new NuGetVersion(1,2,3))},
                false
            },
            new object[]
            {
                "#r Abc 1.2.3-beta1",
                new [] {new AddPackageReferenceCommand("Abc", new NuGetVersion(new Version(1,2,3), "beta1"))},
                false
            },
            new object[]
            {
                "#r Abc",
                new [] {new AddPackageReferenceCommand("Abc", default)},
                false
            },
            // Errors
            new object[]
            {
                "#r Abc 1.2.3 xyz",
                Array.Empty<ICommand>(),
                true
            },
            new object[]
            {
                "#r Abc xyz",
                Array.Empty<ICommand>(),
                true
            },
            new object[]
            {
                "#r   ",
                Array.Empty<ICommand>(),
                true
            },
            new object[]
            {
                "#r",
                Array.Empty<ICommand>(),
                true
            },
        };

        private AddPackageReferenceCommandFactory CreateInstance() =>
            new(_log.Object);
    }
}