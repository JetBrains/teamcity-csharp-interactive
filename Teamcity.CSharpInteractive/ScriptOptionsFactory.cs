// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class ScriptOptionsFactory:
        IScriptOptionsFactory,
        IReferenceRegistry,
        ISettingSetter<LanguageVersion>,
        ISettingSetter<OptimizationLevel>,
        ISettingSetter<WarningLevel>,
        ISettingSetter<CheckOverflow>,
        ISettingSetter<AllowUnsafe>
    {
        internal static readonly ScriptOptions Default = ScriptOptions.Default.AddImports("System");

        private readonly ILog<ScriptOptionsFactory> _log;
        private ScriptOptions _options = Default;

        public ScriptOptionsFactory(IEnvironment environment, ILog<ScriptOptionsFactory> log)
        {
            _log = log;
            _options = _options
                .AddReferences(Path.Combine(environment.GetPath(SpecialFolder.Bin), "Teamcity.CSharpInteractive.Contracts.dll"))
                .AddImports("Teamcity.CSharpInteractive.Contracts");
        }

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

        OptimizationLevel? ISettingSetter<OptimizationLevel>.SetSetting(OptimizationLevel value)
        {
            var prev = _options.OptimizationLevel;
            _options = _options.WithOptimizationLevel(value);
            return prev;
        }

        WarningLevel? ISettingSetter<WarningLevel>.SetSetting(WarningLevel value)
        {
            var prev = (WarningLevel)_options.WarningLevel;
            _options = _options.WithWarningLevel((int)value);
            return prev;
        }

        CheckOverflow? ISettingSetter<CheckOverflow>.SetSetting(CheckOverflow value)
        {
            var prev = _options.CheckOverflow ? CheckOverflow.On : CheckOverflow.Off;
            _options = _options.WithCheckOverflow(value == CheckOverflow.On);
            return prev;
        }

        AllowUnsafe? ISettingSetter<AllowUnsafe>.SetSetting(AllowUnsafe value)
        {
            var prev = _options.AllowUnsafe ? AllowUnsafe.On : AllowUnsafe.Off;
            _options = _options.WithAllowUnsafe(value == AllowUnsafe.On);
            return prev;
        }
    }
}