using NuGet.Versioning;

enum BuildAction
{
    Build,
    CreateImage
}

record Settings(BuildAction Action, string Configuration, NuGetVersion Version);