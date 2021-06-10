namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal readonly struct ErrorId
    {
        public static readonly ErrorId File = new ErrorId("CSI001");
        public static readonly ErrorId Nuget = new ErrorId("CSI002");
        public static readonly ErrorId InvalidScriptDirective = new ErrorId("CSI003");
        public static readonly ErrorId CannotParsePackageVersion = new ErrorId("CSI004");
        public static readonly ErrorId AbnormalProgramTermination = new ErrorId("CSI005");
        public static readonly ErrorId NotSupported = new ErrorId("CSI006");
        public static readonly ErrorId Exception = new ErrorId("CSI007");

        public readonly string Id;

        public ErrorId(string id) => Id = id;

        public override bool Equals(object? obj) => obj is ErrorId other && Id == other.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}