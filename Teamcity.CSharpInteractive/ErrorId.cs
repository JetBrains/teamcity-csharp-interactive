namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal readonly struct ErrorId
    {
        public static readonly ErrorId File = new("CSI001");
        public static readonly ErrorId Nuget = new("CSI002");
        public static readonly ErrorId InvalidScriptDirective = new("CSI003");
        public static readonly ErrorId CannotParsePackageVersion = new("CSI004");
        public static readonly ErrorId AbnormalProgramTermination = new("CSI005");
        public static readonly ErrorId NotSupported = new("CSI006");
        public static readonly ErrorId Exception = new("CSI007");

        public readonly string Id;

        public ErrorId(string id) => Id = id;

        public override bool Equals(object? obj) => obj is ErrorId other && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}