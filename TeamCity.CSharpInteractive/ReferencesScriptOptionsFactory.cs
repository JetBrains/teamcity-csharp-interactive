// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Scripting;

    internal class ReferencesScriptOptionsFactory: IScriptOptionsFactory, IReferenceRegistry
    {
        private readonly ILog<ReferencesScriptOptionsFactory> _log;
        private readonly HashSet<PortableExecutableReference> _references = new();
        
        public ReferencesScriptOptionsFactory(ILog<ReferencesScriptOptionsFactory> log) => _log = log;

        public ScriptOptions Create(ScriptOptions baseOptions) => baseOptions.AddReferences(_references);

        public bool TryRegisterAssembly(string fileName, out string description)
        {
            try
            {
                fileName = Path.GetFullPath(fileName);
                _log.Trace(() => new []{new Text($"Try register the assembly \"{fileName}\".")});
                var reference = MetadataReference.CreateFromFile(fileName);
                description = reference.Display ?? string.Empty;
                _references.Add(reference);
                _log.Trace(() => new []{new Text($"New metadata reference added \"{reference.Display}\".")});
                return true;
            }
            catch(Exception ex)
            {
                description = ex.Message;
            }

            return false;
        }
    }
}