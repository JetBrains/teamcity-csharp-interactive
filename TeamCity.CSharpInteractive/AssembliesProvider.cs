// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

internal class AssembliesProvider : IAssembliesProvider
{
    private readonly IFileSystem _fileSystem;

    public AssembliesProvider(IFileSystem fileSystem) => _fileSystem = fileSystem;

    public IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types) =>
        GetCurrentDomainAssemblies()
            .Concat(GetTypesAssemblies(types))
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Distinct();

    private static IEnumerable<Assembly> GetCurrentDomainAssemblies() => AppDomain.CurrentDomain.GetAssemblies();

    private IEnumerable<Assembly> GetTypesAssemblies(IEnumerable<Type> types) =>
        from assemblyFile in GetAssemblyFiles(types)
        where _fileSystem.IsFileExist(assemblyFile)
        let assembly = LoadAssemblySafely(assemblyFile)
        where assembly != default
        select assembly!;

    private IEnumerable<string> GetAssemblyFiles(IEnumerable<Type> types) => (
            from basePath in GetTypesBasePaths(types)
            from assemblyFile in _fileSystem.EnumerateFileSystemEntries(basePath, "*.dll", SearchOption.TopDirectoryOnly)
            select assemblyFile)
        .Distinct();

    private static IEnumerable<string> GetTypesBasePaths(IEnumerable<Type> types) => (
            from type in types
            let basePath = Path.GetDirectoryName(type.Assembly.Location)
            where basePath != default
            select (string)basePath!)
        .Distinct();

    private Assembly? LoadAssemblySafely(string assemblyFile)
    {
        try
        {
            return Assembly.LoadFrom(assemblyFile);
        }
        catch
        {
            // ignored
        }

        return default;
    }
}