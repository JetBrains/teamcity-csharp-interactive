namespace TeamCity.CSharpInteractive;

internal interface IReferenceRegistry
{
    bool TryRegisterAssembly(string fileName, out string description);
}