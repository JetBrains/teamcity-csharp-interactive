// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;

internal class AssembliesScriptOptionsProvider : IScriptOptionsFactory, IActive
{
    [SuppressMessage("ReSharper", "RedundantNameQualifier")]
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    internal static readonly (string ns, Type? type)[] Refs =
    {
        ("System", typeof(String)),
        ("System.Collections.Generic", typeof(List<string>)),
        ("System.IO", typeof(Path)),
        ("System.Linq", typeof(Enumerable)),
        ("System.Net.Http", typeof(HttpRequestMessage)),
        ("System.Threading", typeof(Thread)),
        ("System.Threading.Tasks", typeof(Task)),
        ("", typeof(IServiceCollection))
    };

    private readonly ILog<AssembliesScriptOptionsProvider> _log;
    private readonly IAssembliesProvider _assembliesProvider;
    private readonly CancellationToken _cancellationToken;
    private readonly Lazy<IEnumerable<Assembly>> _assemblies;

    public AssembliesScriptOptionsProvider(
        ILog<AssembliesScriptOptionsProvider> log,
        IAssembliesProvider assembliesProvider,
        CancellationToken cancellationToken)
    {
        _log = log;
        _assembliesProvider = assembliesProvider;
        _cancellationToken = cancellationToken;
        _assemblies = new Lazy<IEnumerable<Assembly>>(GetAssemblies, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public ScriptOptions Create(ScriptOptions baseOptions) =>
        baseOptions
            .AddReferences(_assemblies.Value)
            .AddImports(Refs.Select(i => i.ns).Where(i => !string.IsNullOrWhiteSpace(i)));

    public IDisposable Activate()
    {
        Task.Run(() => _assemblies.Value, _cancellationToken);
        return Disposable.Empty;
    }

    private IEnumerable<Assembly> GetAssemblies()
    {
        _log.Trace(() => new[] {new Text("Loading assemblies.")});
        var assemblies = _assembliesProvider
            .GetAssemblies(Refs.Where(i => i.type != default).Select(i => i.type!))
            .Where(_ => !_cancellationToken.IsCancellationRequested)
            .Where(i => !string.IsNullOrWhiteSpace(i.Location))
            .ToHashSet();

        _log.Trace(() => new[] {new Text($"{assemblies.Count} assemblies were loaded:")}.Concat(assemblies.Select(i => new[] {Text.NewLine, new Text(i.ToString())}).SelectMany(i => i)).ToArray());
        return assemblies;
    }
}