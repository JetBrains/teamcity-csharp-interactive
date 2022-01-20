// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections;
using Host;

internal class Properties: IProperties
{
    private readonly ILog<Properties> _log;
    private readonly Dictionary<string, string> _props;
        
    public Properties(
        ILog<Properties> log,
        ISettings settings)
    {
        _log = log;
        _props = new Dictionary<string, string>(FilterPairs(settings.ScriptProperties));
    }

    public int Count
    {
        get
        {
            lock (_props)
            {
                return _props.Count;
            }
        }
    }

    public string this[string key]
    {
        get => TryGetValue(key, out var value) ? value : string.Empty;
        set
        {
            lock (_props)
            {
                _log.Trace(() => new []{new Text($"Props[\"{key}\"]=\"{value}\"")});
                if (!string.IsNullOrEmpty(value))
                {
                    _props[key] = value;
                }
                else
                {
                    _log.Trace(() => new []{new Text($"Props.Remove(\"{key}\")")});
                    _props.Remove(key);
                }
            }
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        IReadOnlyList<KeyValuePair<string, string>> props;
        lock (_props)
        {
            props = FilterPairs(_props).ToList().AsReadOnly();
        }

        return props.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool TryGetValue(string key, out string value)
    {
        lock (_props)
        {
            _props.TryGetValue(key, out var curValue);
            value = curValue ?? string.Empty;
            _log.Trace(() => new []{new Text($"Props[\"{key}\"] returns \"{curValue ?? "empty"}\"")});
            return !string.IsNullOrEmpty(value);
        }
    }
        
    private static IEnumerable<KeyValuePair<string, string>> FilterPairs(IEnumerable<KeyValuePair<string, string>> pairs) =>
        pairs.Where(i => !string.IsNullOrWhiteSpace(i.Key) && !string.IsNullOrEmpty(i.Value));
}