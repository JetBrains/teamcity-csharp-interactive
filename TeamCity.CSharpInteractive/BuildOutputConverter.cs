// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using System.Linq;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages.Read;

internal class BuildOutputConverter : IBuildOutputConverter
{
    private readonly IServiceMessageParser _serviceMessageParser;

    public BuildOutputConverter(IServiceMessageParser serviceMessageParser) => _serviceMessageParser = serviceMessageParser;

    public IEnumerable<BuildMessage> Convert(in Output output, IBuildResult result) =>
        Convert(output.StartInfo, output.Line, result)
            .DefaultIfEmpty(new BuildMessage(output.IsError ? BuildMessageState.Error : BuildMessageState.Info, default, output.Line));

    private IEnumerable<BuildMessage> Convert(IStartInfo startInfo, string line, IBuildResult result)
    {
        foreach (var message in _serviceMessageParser.ParseServiceMessages(line))
        {
            yield return new BuildMessage(BuildMessageState.ServiceMessage, message);
            foreach (var buildMessage in result.ProcessMessage(startInfo, message))
            {
                yield return buildMessage;
            }
        }
    }
}