namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis.CSharp;

internal interface IScriptSubmissionAnalyzer
{
    bool IsCompleteSubmission(string script, CSharpParseOptions parseOptions);
}