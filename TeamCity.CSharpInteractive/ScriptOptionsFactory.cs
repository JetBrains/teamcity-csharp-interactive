// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;

    internal class ScriptOptionsFactory:
        MetadataReferenceResolver,
        IScriptOptionsFactory,
        IReferenceRegistry,
        ISettingSetter<LanguageVersion>,
        ISettingSetter<OptimizationLevel>,
        ISettingSetter<WarningLevel>,
        ISettingSetter<CheckOverflow>,
        ISettingSetter<AllowUnsafe>
    {
        internal static readonly ScriptOptions Default = ScriptOptions.Default
            .AddReferences(AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic && !i.ReflectionOnly))
            .AddImports("System", "TeamCity.CSharpInteractive.Contracts");

        private readonly ILog<ScriptOptionsFactory> _log;
        private ScriptOptions _options = Default;

        public ScriptOptionsFactory(ILog<ScriptOptionsFactory> log)
        {
            _log = log;
            _options = _options.WithMetadataResolver(this);
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

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string? baseFilePath, MetadataReferenceProperties properties)
        {
            try
            {
                _log.Trace($"Resoling reference \"{reference}\".");
                var assembly = Assembly.ReflectionOnlyLoad(reference);
                _log.Trace($"The reference was resolved as the assembly {assembly.GetName()}.");
                return ImmutableArray.Create(MetadataReference.CreateFromFile(assembly.Location));
            }
            catch (Exception ex)
            {
                _log.Trace($"The reference \"{reference}\" was not resolved: {ex.Message}");
                return ImmutableArray<PortableExecutableReference>.Empty;
            }
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && _options.Equals(((ScriptOptionsFactory)obj)._options);
        }

        public override int GetHashCode() => _options.GetHashCode();
    }
}