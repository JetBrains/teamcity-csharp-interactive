// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class DefaultBuildMessagesProcessor : IBuildMessagesProcessor
{
    private readonly ITeamCitySettings _teamCitySettings;
    private readonly IProcessOutputWriter _processOutputWriter;
    private readonly IBuildMessageLogWriter _buildMessageLogWriter;

    public DefaultBuildMessagesProcessor(
        ITeamCitySettings teamCitySettings,
        IProcessOutputWriter processOutputWriter,
        IBuildMessageLogWriter buildMessageLogWriter)
    {
        _teamCitySettings = teamCitySettings;
        _processOutputWriter = processOutputWriter;
        _buildMessageLogWriter = buildMessageLogWriter;
    }

    public void ProcessMessages(in Output output, IEnumerable<BuildMessage> messages, Action<BuildMessage> nextHandler)
    {
        var curMessages = messages.ToArray();
        if (_teamCitySettings.IsUnderTeamCity && curMessages.Any(i => i.State == BuildMessageState.ServiceMessage))
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