// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[ExcludeFromCodeCoverage]
internal class AssembliesProvider : IAssembliesProvider
{
    public IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types) => 
        GetAssemblies(AppDomain.CurrentDomain.GetAssemblies()).Concat(types.Select(i => i.Assembly)).Where(i => !i.IsDynamic).Distinct();
    
    private static IEnumerable<Assembly> GetAssemblies(IEnumerable<Assembly> assemblies)
    {
        var processed = new HashSet<Assembly>(assemblies);
        var processing = new Queue<Assembly>(processed);
        while (processing.TryDequeue(out var assembly))
        {
            if (!assembly.IsDynamic)
            {
                yield return assembly;
            }

            foreach (var refAssemblyName in assembly.GetReferencedAssemblies())
            {
                try
                {
                    var refAssembly = Assembly.Load(refAssemblyName);
                    if (processed.Add(refAssembly))
                    {
                        processing.Enqueue(refAssembly);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}