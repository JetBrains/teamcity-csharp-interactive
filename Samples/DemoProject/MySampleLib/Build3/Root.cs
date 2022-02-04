class Root
{
    private readonly Build _build;
    private readonly CreateImage _createImage;

    public Root(Build build,
        CreateImage createImage)
    {
        _build = build;
        _createImage = createImage;
    }

    public Task<Optional<string>> RunAsync() =>
        Tools.GetProperty("target", "Build") switch
        {
            "Build" => _build.RunAsync(),
            "CreateImage" => _createImage.RunAsync(),
            _ => throw new ArgumentOutOfRangeException("target")
        };
}