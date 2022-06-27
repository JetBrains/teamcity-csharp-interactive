// ReSharper disable MemberCanBeProtected.Global
namespace TeamCity.CSharpInteractive.Tests.UsageScenarios;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HostApi;
using Environment = System.Environment;

public class BaseScenario : IHost, IDisposable
{
    private readonly string _tempDir;
    private readonly string _prevCurDir;

    public BaseScenario()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()[..4]);
        Directory.CreateDirectory(_tempDir);
        _prevCurDir = Environment.CurrentDirectory;
        Environment.CurrentDirectory = _tempDir;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public IHost Host => this;

    public IReadOnlyList<string> Args { get; } = new List<string>
    {
        "Arg1",
        "Arg2"
    };

    public IProperties Props { get; } = new Properties();

    public T GetService<T>() => Composer.Resolve<T>();

    public void WriteLine() => Composer.Resolve<IHost>().WriteLine();

    public void WriteLine<T>(T line, Color color = Color.Default) => Composer.Resolve<IHost>().WriteLine(line, color);

    public void Error(string? error, string? errorId = default) => Composer.Resolve<IHost>().Error(error, errorId);

    public void Warning(string? warning) => Composer.Resolve<IHost>().Warning(warning);

    public void Info(string? text) => Composer.Resolve<IHost>().Info(text);

    public void Trace(string? trace, string? origin = default) => Composer.Resolve<IHost>().Trace(trace, origin);

    private class Properties : IProperties
    {
        private readonly Dictionary<string, string> _dict = new()
        {
            {"TEAMCITY_VERSION", "2021.2"},
            {"TEAMCITY_PROJECT_NAME", "Samples"}
        };

        public int Count => _dict.Count;

        public string this[string key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _dict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value) => _dict.TryGetValue(key, out value!);
    }

    void IDisposable.Dispose()
    {
        try
        {
            Directory.Delete(_tempDir, true);
        }
        catch
        {
            // ignored
        }
        
        Environment.CurrentDirectory = _prevCurDir;
    }
}