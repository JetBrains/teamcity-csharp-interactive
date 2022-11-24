// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class DefaultBuildMessagesProcessor : IBuildMessagesProcessor
{
    private readonly ICISettings _ciSettings;
    private readonly IProcessOutputWriter _processOutputWriter;
    private readonly IBuildMessageLogWriter _buildMessageLogWriter;

    public DefaultBuildMessagesProcessor(
        ICISettings ciSettings,
        IProcessOutputWriter processOutputWriter,
        IBuildMessageLogWriter buildMessageLogWriter)
    {
        _ciSettings = ciSettings;
        _processOutputWriter = processOutputWriter;
        _buildMessageLogWriter = buildMessageLogWriter;
    }

    public void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler)
    {
        var curMessages = messages.ToArray();
        if (_ciSettings.CIType == CIType.TeamCity && curMessages.Any(i => i.State == BuildMessageState.ServiceMessage))
        {
            _processOutputWriter.Write(output);
        }
        else
        {
            foreach (var buildMessage in curMessages)
            {
                _buildMessageLogWriter.Write(buildMessage);
            }
        }
    }
}