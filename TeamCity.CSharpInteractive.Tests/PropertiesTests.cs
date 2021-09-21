namespace TeamCity.CSharpInteractive.Tests
{
    using System.Collections.Generic;
    using Moq;
    using Shouldly;
    using Xunit;

    public class PropertiesTests
    {
        private readonly Mock<ILog<Properties>> _log;
        private readonly Mock<ISettings> _settings;
        private readonly Dictionary<string, string> _scriptProperties = new();

        public PropertiesTests()
        {
            _log = new Mock<ILog<Properties>>();
            _settings = new Mock<ISettings>();
            _settings.SetupGet(i => i.ScriptProperties).Returns(_scriptProperties);
        }
        
        [Fact]
        public void ShouldInit()
        {
            // Given
            _scriptProperties["Abc"] = "Xyz";
            _scriptProperties["11"] = " ";
            _scriptProperties["Xyz"] = string.Empty;
            var props = CreateInstance();

            // When

            // Then
            props.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldSupportIndexedGetter()
        {
            // Given
            _scriptProperties["Abc"] = "Xyz";
            var props = CreateInstance();

            // When
            var val = props["Abc"];

            // Then
            val.ShouldBe("Xyz");
        }
        
        [Fact]
        public void ShouldSupportTryGetValue()
        {
            // Given
            _scriptProperties["Abc"] = "Xyz";
            var props = CreateInstance();

            // When
            props.TryGetValue("Abc", out var val).ShouldBeTrue();

            // Then
            val.ShouldBe("Xyz");
        }
        
        [Fact]
        public void ShouldSupportIndexedSetter()
        {
            // Given
            var props = CreateInstance();

            // When
            props["Abc"] = "Xyz";
            var val = props["Abc"];

            // Then
            val.ShouldBe("Xyz");
            props.Count.ShouldBe(1);
        }
        
        [Fact]
        public void ShouldGetEmptyStringWhenNoValue()
        {
            // Given
            var props = CreateInstance();

            // When
            var val = props["Abc"];

            // Then
            val.ShouldBe(string.Empty);
        }
        
        [Fact]
        public void ShouldSupportTryGetValueWhenNoValue()
        {
            // Given
            var props = CreateInstance();

            // When
            var result = props.TryGetValue("Abc", out _);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void ShouldEnumeratePairs()
        {
            // Given
            _scriptProperties["Abc"] = "Xyz";
            _scriptProperties["1"] = "2";
            var props = CreateInstance();

            // When
            
            // Then
            props.ShouldBe(new []
            {
                new KeyValuePair<string, string>("Abc", "Xyz"),
                new KeyValuePair<string, string>("1", "2")
            });
        }
        
        [Fact]
        public void ShouldRemoveStringWhenSetEmptyValue()
        {
            // Given
            _scriptProperties["Abc"] = "Xyz";
            var props = CreateInstance();

            // When
            props["Abc"] = string.Empty;

            // Then
            props["Abc"].ShouldBe(string.Empty);
            props.Count.ShouldBe(0);
        }

        private Properties CreateInstance() =>
            new(_log.Object, _settings.Object);
    }
}