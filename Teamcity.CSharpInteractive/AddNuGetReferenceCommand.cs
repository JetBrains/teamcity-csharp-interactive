namespace Teamcity.CSharpInteractive
{
    using System;
    using NuGet.Versioning;

    internal class AddNuGetReferenceCommand: ICommand
    {
        public readonly string PackageId;
        public readonly NuGetVersion? Version;

        public AddNuGetReferenceCommand(string packageId, NuGetVersion? version)
        {
            PackageId = packageId;
            Version = version;
        }
        
        public string Name => $"Package {PackageId} {Version?.ToString() ?? "latest"}";

        public bool Internal => false;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            AddNuGetReferenceCommand other = (AddNuGetReferenceCommand) obj;
            return PackageId == other.PackageId && Equals(Version?.ToString(), other.Version?.ToString());
        }

        public override int GetHashCode() => HashCode.Combine(PackageId, Version?.ToString());
    }
}