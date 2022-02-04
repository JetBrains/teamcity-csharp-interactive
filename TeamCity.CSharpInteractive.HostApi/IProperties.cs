namespace HostApi;

using System.Diagnostics.Contracts;

public interface IProperties : IEnumerable<KeyValuePair<string, string>>
{
    [Pure]
    int Count { get; }

    string this[string key] { get; set; }

    bool TryGetValue(string key, out string value);
}