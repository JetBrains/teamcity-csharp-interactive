namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NuGet.Versioning;

    [ExcludeFromCodeCoverage]
    internal class AddNuGetReferenceCommand: ICommand
    {
        public readonly string PackageId;
        public readonly VersionRange? VersionRange;

        public AddNuGetReferenceCommand(string packageId, VersionRange? versionRange)
        {
            PackageId = packageId;
            VersionRange = versionRange;
        }
        
        public string Name => $"Package {PackageId} {VersionRange?.ToString() ?? "latest"}";

        public bool Internal => false;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            var other = (AddNuGetReferenceCommand) obj;
            return PackageId == other.PackageId && Equals(VersionRange?.ToString(), other.VersionRange?.ToString());
        }

        public override int GetHashCode() => HashCode.Combine(PackageId, VersionRange?.ToString());
    }
}