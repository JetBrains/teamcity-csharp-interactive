// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    [ExcludeFromCodeCoverage]
    internal class ScriptSubmissionAnalyzer : IScriptSubmissionAnalyzer
    {
        public bool IsCompleteSubmission(string script, CSharpParseOptions parseOptions) => 
            SyntaxFactory.IsCompleteSubmission(SyntaxFactory.ParseSyntaxTree(script, parseOptions));
    }
}