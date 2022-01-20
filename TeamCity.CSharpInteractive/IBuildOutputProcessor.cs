namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using Cmd;
using DotNet;

internal interface IBuildOutputProcessor
{
    IEnumerable<BuildMessage> Convert(in Output output, IBuildResult result);
}