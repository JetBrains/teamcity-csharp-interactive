namespace Teamcity.CSharpInteractive
{
    using NuGet.Versioning;

    internal class AddPackageReferenceCommand: ICommand
    {
        public readonly string PackageId;
        public readonly NuGetVersion? Version;

        public AddPackageReferenceCommand(string packageId, NuGetVersion? version)
        {
            PackageId = packageId;
            Version = version;
        }
        
        public string Name => $"Package {PackageId} {Version?.ToString() ?? "latest"}";
    }
}