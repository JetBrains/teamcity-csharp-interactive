namespace TeamCity.CSharpInteractive;

internal interface IReferenceRegistry
{
    bool TryRegisterAssembly(string assemblyPath, out string description);
}