// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[ExcludeFromCodeCoverage]
internal class AssembliesProvider : IAssembliesProvider
{
    public IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types) => 
        AppDomain.CurrentDomain.GetAssemblies().Concat(types.Select(i => i.Assembly)).Where(i => !i.IsDynamic).Distinct();
}