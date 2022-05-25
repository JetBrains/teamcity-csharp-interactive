using HostApi;
// ReSharper disable CheckNamespace
// ReSharper disable ArrangeTypeModifiers

interface IRoot
{
    Task<string> BuildAsync();
}

class Root : IRoot
{
    private readonly IProperties _properties;
    private readonly IBuild _build;
    private readonly ICreateImage _createImage;

    public Root(
        IProperties properties,
        IBuild build,
        ICreateImage createImage)
    {
        _properties = properties;
        _build = build;
        _createImage = createImage;
    }

    public Task<string> BuildAsync() =>
        Property.Get(_properties, "target", "Build") switch
        {
            "Build" => _build.BuildAsync(),
            "CreateImage" => _createImage.BuildAsync(),
            _ => throw new ArgumentOutOfRangeException()
        };
}