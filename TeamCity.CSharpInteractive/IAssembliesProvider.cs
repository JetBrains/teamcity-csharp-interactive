namespace TeamCity.CSharpInteractive;

using System.Reflection;

internal interface IAssembliesProvider
{
    IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types);
}