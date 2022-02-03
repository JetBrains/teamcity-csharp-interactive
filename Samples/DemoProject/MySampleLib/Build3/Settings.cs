using NuGet.Versioning;

enum Target
{
    Build,
    CreateImage
}

record Settings(Target Action, string Configuration, NuGetVersion Version);

readonly record struct Optional<T>(T Value, bool HasValue = true)
{
    public static implicit operator Optional<T>(T value) => new(value);

    public override string ToString() => HasValue ? Value?.ToString() ?? "null" : "unspecified";
}