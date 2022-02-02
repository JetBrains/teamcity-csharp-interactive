class Root
{
    private readonly Settings _settings;
    private readonly Build _build;
    private readonly CreateImage _createImage;

    public Root(
        Settings settings,
        Build build,
        CreateImage createImage)
    {
        _settings = settings;
        _build = build;
        _createImage = createImage;
    }

    public Task<Optional<string>> RunAsync() =>
        _settings.Action switch
        {
            BuildAction.Build => _build.RunAsync(),
            BuildAction.CreateImage => _createImage.RunAsync(),
            _ => throw new ArgumentOutOfRangeException()
        };
}