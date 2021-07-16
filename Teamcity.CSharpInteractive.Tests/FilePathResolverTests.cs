namespace Teamcity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Moq;
    using Shouldly;
    using Xunit;

    public class FilePathResolverTests
    {
        private readonly Mock<ILog<FilePathResolver>> _log;
        private readonly Mock<IEnvironment> _environment;
        private readonly Mock<IFileSystem> _fileSystem;
        private readonly List<Text> _errors = new();
        
        public FilePathResolverTests()
        {
            _log = new Mock<ILog<FilePathResolver>>();
            _log.Setup(i => i.Error(It.IsAny<ErrorId>(),It.IsAny<Text[]>())).Callback<ErrorId, Text[]>((_, text) => _errors.AddRange(text));
            
            _environment = new Mock<IEnvironment>();
            _fileSystem = new Mock<IFileSystem>();

            _fileSystem.Setup(i => i.IsPathRooted(It.IsAny<string>())).Returns(false);
            _fileSystem.Setup(i => i.IsPathRooted("Rooted")).Returns(true);
            _environment.Setup(i => i.GetPath(SpecialFolder.Script)).Returns("sc");
            _environment.Setup(i => i.GetPath(SpecialFolder.Working)).Returns("wd");
            _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(i => i == "wd/Existing1".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
            _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(i => i == "sc/Existing2".Replace('/', Path.DirectorySeparatorChar)))).Returns(true);
            _fileSystem.Setup(i => i.IsFileExist(It.Is<string>(i => i == "wd/NotExisting".Replace('/', Path.DirectorySeparatorChar)))).Returns(false);
        }

        [Theory]
        [InlineData(null, "", false)]
        [InlineData("", "", false)]
        [InlineData(" ", "", false)]
        [InlineData("    ", "", false)]
        [InlineData("Rooted", "", false)]
        [InlineData("Existing1", "wd/Existing1", false)]
        [InlineData("Existing2", "sc/Existing2", false)]
        [InlineData("NotExisting", "", true)]
        public void ShouldResolveFullAssemblyPath(string? assemblyPath, string expectedAssemblyPath, bool hasErrors)
        {
            // Given
            var resolver = CreateInstance();

            // When
            var actualResult = resolver.TryResolve(assemblyPath, out var actualAssemblyPath);

            // Then
            actualResult.ShouldBe(expectedAssemblyPath != string.Empty);
            actualAssemblyPath.ShouldBe(expectedAssemblyPath.Replace('/', Path.DirectorySeparatorChar));
            _errors.Any().ShouldBe(hasErrors);
        }

        private FilePathResolver CreateInstance() =>
            new(_log.Object, _environment.Object, _fileSystem.Object);
    }
}