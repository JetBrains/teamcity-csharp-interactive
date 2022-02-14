namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

internal interface IRuntimeExplorer
{
    bool TryFindRuntimeAssembly(string assemblyPath, [MaybeNullWhen(false)] out string runtimeAssemblyPath);
}