namespace Teamcity.CSharpInteractive
{
    internal interface IAssemblyPathResolver
    {
        bool TryResolve(string? assemblyPath, out string fullAssemblyPath);
    }
}