namespace TeamCity.CSharpInteractive;

using Cmd;
using CSharpInteractive;
using DotNet;

internal interface IBuildOutputProcessor
{
    IEnumerable<BuildMessage> Convert(in Output output, IBuildContext context);
}