// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public readonly struct NuGetPackage
    {
        public readonly string Name;
        public readonly Version Version;
        public readonly string Type;
        public readonly string Path;
        public readonly string Sha512;

        public NuGetPackage(
            string name,
            Version version,
            string type,
            string path,
            string sha512)
        {
            Name = name;
            Version = version;
            Type = type;
            Path = path;
            Sha512 = sha512;
        }

        public override bool Equals(object? obj) =>
            obj is NuGetPackage other && Name == other.Name && Version.Equals(other.Version) && Type == other.Type && Path == other.Path && Sha512 == other.Sha512;

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Version.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ Path.GetHashCode();
                hashCode = (hashCode * 397) ^ Sha512.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"{Type} {Name} v.{Version} Sha512:{Sha512} at {Path}";
    }
}