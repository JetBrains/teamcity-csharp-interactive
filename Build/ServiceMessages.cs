// ReSharper disable CheckNamespace

using JetBrains.TeamCity.ServiceMessages;
// ReSharper disable ArrangeTypeModifiers

class ServiceMessage : IServiceMessage
{
    private readonly Dictionary<string, string> _values;

    protected ServiceMessage(string name, params (string key, string value)[] tags)
    {
        Name = name;
        _values = tags.ToDictionary(i => i.key, i => i.value);
    }

    public string Name { get; }

    public string? DefaultValue => default;

    public IEnumerable<string> Keys => _values.Keys;

    public string? GetValue(string key) => _values.TryGetValue(key, out var value) ? value : default;
}

class DotNetCoverageServiceMessage: ServiceMessage
{
    public DotNetCoverageServiceMessage()
        : base(
            "dotNetCoverage",
            ("dotcover_home", Props["dotCoverHome"]))
    { }
}

class ImportDataDotCoverReportServiceMessage: ServiceMessage
{
    public ImportDataDotCoverReportServiceMessage(string path)
        : base(
            "importData",
            ("type", "dotNetCoverage"),
            ("tool", "dotcover"),
            ("path", path))
    { }
}