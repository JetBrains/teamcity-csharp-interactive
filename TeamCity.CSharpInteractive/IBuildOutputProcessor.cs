namespace TeamCity.CSharpInteractive;

using Script.Cmd;
using Script.DotNet;

internal interface IBuildOutputProcessor
{
    IEnumerable<BuildMessage> Convert(in Output output, IBuildContext context);
}