namespace Teamcity.CSharpInteractive.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using Shouldly;
    using Xunit;

    public class InitialStateCodeSourceTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldGenerateInitialCode(string[] args, Dictionary<string,string> props, string[] checkLines)
        {
            // Given
            var source = new InitialStateCodeSource
            {
                ScriptArguments = args,
                ScriptProperties = props
            };

            // When
            RunScript(source.Concat(checkLines));

            // Then
        }
        
        public static IEnumerable<object?[]> Data => new List<object?[]>
        {
            new object[]
            {
                new [] { "Abc", "Xyz" },
                new Dictionary<string, string>(),
                new[] { "Console.WriteLine(Args[0]);", "Console.WriteLine(Args[1]);" }
            },
            
            new object[]
            {
                Array.Empty<string>(),
                new Dictionary<string, string>{ {"Key1", "Val1"}, {"key2", "Val2"} },
                new[] { "Console.WriteLine(Props[\"Key1\"]);", "Console.WriteLine(Props[\"key2\"]);" }
            },
            
            new object[]
            {
                new [] { "Abc" },
                new Dictionary<string, string>{ {"Key1", "Val1"} },
                new[] { "Console.WriteLine(Args[0]);", "Console.WriteLine(Props[\"Key1\"]);" }
            }
        };

        private static void RunScript(IEnumerable<string> lines)
        {
            ScriptState<object>? scriptState = null;
            foreach (var line in lines)
            {
                var result = true;
                scriptState =
                    (scriptState ?? CSharpScript.RunAsync(string.Empty, ScriptOptionsFactory.Default).Result)
                    .ContinueWithAsync(
                        line,
                        ScriptOptionsFactory.Default,
                        _ =>
                        {
                            result = false;
                            return true;
                        })
                    .Result;
                
                result.ShouldBeTrue($"Error running: {line}");
            }
        }
    }
}