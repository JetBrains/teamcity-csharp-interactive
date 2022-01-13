namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using Cmd;
using Dotnet;

internal interface IBuildOutputConverter
{
    IEnumerable<BuildMessage> Convert(in Output output, IBuildResult result);
}