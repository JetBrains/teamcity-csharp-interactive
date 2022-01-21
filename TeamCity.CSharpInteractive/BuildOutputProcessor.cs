// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using JetBrains.TeamCity.ServiceMessages.Read;
using Script.Cmd;
using Script.DotNet;

internal class BuildOutputProcessor : IBuildOutputProcessor
{
    private readonly IServiceMessageParser _serviceMessageParser;

    public BuildOutputProcessor(IServiceMessageParser serviceMessageParser)
    {
        _serviceMessageParser = serviceMessageParser;
    }

    public IEnumerable<BuildMessage> Convert(in Output output, IBuildContext context)
    {
        var messages = new List<BuildMessage>();
        foreach (var message in _serviceMessageParser.ParseServiceMessages(output.Line))
        {
            messages.Add(new BuildMessage(BuildMessageState.ServiceMessage, message));
            messages.AddRange(context.ProcessMessage(output, message));
        }

        if (!messages.Any()) messages.AddRange(context.ProcessOutput(output));

        return messages;
    }
}