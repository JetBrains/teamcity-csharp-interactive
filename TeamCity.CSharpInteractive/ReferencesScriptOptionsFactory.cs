// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

internal class ReferencesScriptOptionsFactory : IScriptOptionsFactory, IReferenceRegistry
{
    private readonly ILog<ReferencesScriptOptionsFactory> _log;
    private readonly IRuntimeExplorer _runtimeExplorer;
    private readonly HashSet<PortableExecutableReference> _references = new();
    
    public ReferencesScriptOptionsFactory(
        ILog<ReferencesScriptOptionsFactory> log,
        IRuntimeExplorer runtimeExplorer)
    {
        _log = log;
        _runtimeExplorer = runtimeExplorer;
    }

    public ScriptOptions Create(ScriptOptions baseOptions) => baseOptions.AddReferences(_references);

    public bool TryRegisterAssembly(string assemblyPath, out string description)
    {
        try
        {
            assemblyPath = Path.GetFullPath(assemblyPath);
            if (_runtimeExplorer.TryFindRuntimeAssembly(assemblyPath, out var runtimeAssemblyPath))
            {
                AddRef(runtimeAssemblyPath, out description);
            }

            AddRef(assemblyPath, out description);
            return true;
        }
        catch (Exception ex)
        {
            description = ex.Message;
        }

        return false;
    }

    private void AddRef(string fileName, out string description)
    {
        _log.Trace(() => new[] {new Text($"Try register the assembly \"{fileName}\".")});
        var reference = MetadataReference.CreateFromFile(fileName);
        description = reference.Display ?? string.Empty;
        _references.Add(reference);
        _log.Trace(() => new[] {new Text($"New metadata reference added \"{reference.Display}\".")});
    }
}