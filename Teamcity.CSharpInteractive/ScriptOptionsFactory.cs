namespace Teamcity.CSharpInteractive
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class ScriptOptionsFactory : IScriptOptionsFactory, IReferenceRegistry, ISettingSetter<LanguageVersion>
    {
        private readonly ILog<ScriptOptionsFactory> _log;
        internal static readonly ScriptOptions Default =
            ScriptOptions.Default
                .AddImports("System")
                .WithLanguageVersion(LanguageVersion.Latest)
                .WithOptimizationLevel(OptimizationLevel.Release);

        private ScriptOptions _options = Default;

        public ScriptOptionsFactory(ILog<ScriptOptionsFactory> log) => _log = log;

        public ScriptOptions Create() => _options;
        
        public bool TryRegisterAssembly(string fileName, out string description)
        {
            try
            {
                _log.Trace($"Try register the assembly \"{fileName}\".");
                var reference = MetadataReference.CreateFromFile(fileName);
                description = reference.Display ?? string.Empty;
                _options = _options.AddReferences(reference);
                _log.Trace($"New metadata reference added \"{reference.Display}\".");
                return true;
            }
            catch(Exception ex)
            {
                description = ex.Message;
            }

            return false;
        }

        LanguageVersion? ISettingSetter<LanguageVersion>.SetSetting(LanguageVersion value)
        {
            _options = _options.WithLanguageVersion(value);
            return default;
        }
    }
}