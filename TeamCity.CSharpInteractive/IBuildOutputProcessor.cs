namespace TeamCity.CSharpInteractive;

using HostApi;

internal interface IBuildOutputProcessor
{
    IEnumerable<BuildMessage> Convert(in Output output, IBuildContext context);
}