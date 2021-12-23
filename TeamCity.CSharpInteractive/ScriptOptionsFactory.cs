// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
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
        private readonly ILog<ScriptOptionsFactory> _log;
        private readonly CancellationToken _cancellationToken;
        private readonly ManualResetEventSlim _isReady = new(false);
        private ScriptOptions _options = ScriptOptions.Default;
        private static int _assembliesWereLoaded;

        public ScriptOptionsFactory(
            ILog<ScriptOptionsFactory> log,
            CancellationToken cancellationToken)
        {
            _log = log;
            _cancellationToken = cancellationToken;
            Task.Run(ConfigureOptions, cancellationToken);
        }

        private ScriptOptions Options
        {
            get
            {
                _isReady.Wait(_cancellationToken);
                return _options;
            }
        }

        public ScriptOptions Create() => Options;

        public bool TryRegisterAssembly(string fileName, out string description)
        {
            try
            {
                fileName = Path.GetFullPath(fileName);
                _log.Trace(() => new []{new Text($"Try register the assembly \"{fileName}\".")});
                var reference = MetadataReference.CreateFromFile(fileName);
                description = reference.Display ?? string.Empty;
                _options = Options.AddReferences(reference);
                _log.Trace(() => new []{new Text($"New metadata reference added \"{reference.Display}\".")});
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
            _options = Options.WithLanguageVersion(value);
            return default;
        }

        OptimizationLevel? ISettingSetter<OptimizationLevel>.SetSetting(OptimizationLevel value)
        {
            var prev = Options.OptimizationLevel;
            _options = Options.WithOptimizationLevel(value);
            return prev;
        }

        WarningLevel? ISettingSetter<WarningLevel>.SetSetting(WarningLevel value)
        {
            var prev = (WarningLevel)Options.WarningLevel;
            _options = Options.WithWarningLevel((int)value);
            return prev;
        }

        CheckOverflow? ISettingSetter<CheckOverflow>.SetSetting(CheckOverflow value)
        {
            var prev = Options.CheckOverflow ? CheckOverflow.On : CheckOverflow.Off;
            _options = Options.WithCheckOverflow(value == CheckOverflow.On);
            return prev;
        }

        AllowUnsafe? ISettingSetter<AllowUnsafe>.SetSetting(AllowUnsafe value)
        {
            var prev = Options.AllowUnsafe ? AllowUnsafe.On : AllowUnsafe.Off;
            _options = Options.WithAllowUnsafe(value == AllowUnsafe.On);
            return prev;
        }
        
        private void LoadAssembliesFromPathOfType<T>(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _assembliesWereLoaded) != 1)
            {
                return;
            }
            
            var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
            if (basePath == null)
            {
                _log.Warning($"Cannot get a path of {typeof(T).Assembly}.");
                return;
            }
            
            foreach (var assemblyFile in Directory.GetFiles(basePath, "*.dll"))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    Assembly.LoadFrom(assemblyFile);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void ConfigureOptions()
        {
            try
            {
                _log.Trace(() => new []{new Text("Loading assemblies.")});
                LoadAssembliesFromPathOfType<string>(_cancellationToken);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic && !string.IsNullOrWhiteSpace(i.Location)).ToList();
                _log.Trace(() => new [] {new Text($"{assemblies.Count} assemblies were loaded:")}.Concat(assemblies.Select(i => new [] {Text.NewLine, new Text(i.ToString())}).SelectMany(i => i)).ToArray());
                _log.Trace(() => new []{new Text("Add references.")});
                foreach (var assembly in assemblies)
                {
                    _options = _options.AddReferences(assembly);
                }
                
                _options = _options.AddImports("System", "TeamCity.CSharpInteractive.Contracts");
            }
            finally
            {
                _isReady.Set();
            }
        }
    }
}