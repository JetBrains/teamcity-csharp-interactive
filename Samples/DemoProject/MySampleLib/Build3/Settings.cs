using NuGet.Versioning;

record Settings(string Configuration, NuGetVersion Version);

readonly record struct Optional<T>(T Value, bool HasValue = true)
{
    public static implicit operator Optional<T>(T value) => new(value);

    public override string ToString() => HasValue ? Value?.ToString() ?? "null" : "unspecified";
}