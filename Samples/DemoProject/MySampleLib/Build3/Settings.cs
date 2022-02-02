using NuGet.Versioning;

enum BuildAction
{
    Build,
    CreateImage
}

record Settings(BuildAction Action, string Configuration, NuGetVersion Version);

readonly record struct Optional<T>(T Value, bool HasValue = true)
{
    public static implicit operator Optional<T>(T value) => new(value);

    public override string ToString() => HasValue ? Value?.ToString() ?? "null" : "unspecified";
}