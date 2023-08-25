// ReSharper disable UnusedParameter.Global
namespace HostApi;

using JetBrains.TeamCity.ServiceMessages;

public interface ICommandLine
{
    IStartInfo GetStartInfo(IHost host);

    void PreRun(IHost host) {}

    IEnumerable<IServiceMessage> GetNonStdStreamsServiceMessages(IHost host) => Enumerable.Empty<IServiceMessage>();
}
