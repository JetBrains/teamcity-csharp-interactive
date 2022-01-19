// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using System.Linq;
using Cmd;
using Dotnet;
using JetBrains.TeamCity.ServiceMessages.Read;

internal class BuildOutputProcessor : IBuildOutputProcessor
{
    private readonly IServiceMessageParser _serviceMessageParser;

    public BuildOutputProcessor(IServiceMessageParser serviceMessageParser) => _serviceMessageParser = serviceMessageParser;

    public IEnumerable<BuildMessage> Convert(in Output output, IBuildResult result)
    {
        var messages = new List<BuildMessage>();
        foreach (var message in _serviceMessageParser.ParseServiceMessages(output.Line))
        {
            messages.Add(new BuildMessage(BuildMessageState.ServiceMessage, message));
            messages.AddRange(result.ProcessMessage(output, message));
        }

        if (!messages.Any())
        {
            messages.AddRange(result.ProcessOutput(output));
        }

        return messages;
    }
}