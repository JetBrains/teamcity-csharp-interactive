namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal readonly record struct ErrorId(string Id)
{
    public static readonly ErrorId Unhandled = new("CSI000");
    public static readonly ErrorId File = new("CSI001");
    public static readonly ErrorId NuGet = new("CSI002");
    public static readonly ErrorId CannotParsePackageVersion = new("CSI003");
    public static readonly ErrorId AbnormalProgramTermination = new("CSI004");
    public static readonly ErrorId NotSupported = new("CSI005");
    public static readonly ErrorId Exception = new("CSI006");
    public static readonly ErrorId UncompletedScript = new("CSI007");
    public static readonly ErrorId Build = new("CSI008");
    public static readonly ErrorId Process = new("CSI009");
}