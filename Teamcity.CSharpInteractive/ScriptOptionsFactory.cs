namespace Teamcity.CSharpInteractive
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class ScriptOptionsFactory : IScriptOptionsFactory
    {
        internal static readonly ScriptOptions Default = 
            ScriptOptions.Default
            .AddImports("System")
            .WithLanguageVersion(LanguageVersion.Latest)
            .WithOptimizationLevel(OptimizationLevel.Release);

        public ScriptOptions Create() => Default;
    }
}