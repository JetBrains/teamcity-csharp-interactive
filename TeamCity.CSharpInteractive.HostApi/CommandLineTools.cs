namespace HostApi;

using JetBrains.TeamCity.ServiceMessages;

public static class CommandLineTools
{
    public static CommandLine AsCommandLine(this string executable, params string[] args) => new(executable, args);
    
    public static ICommandLine Customize(this ICommandLine baseCommandLine, Func<CommandLine, ICommandLine> customizer) => 
        new CustomCommandLine(baseCommandLine, customizer);

    private class CustomCommandLine: ICommandLine
    {
        private readonly ICommandLine _baseCommandLine;
        private readonly Func<CommandLine, ICommandLine> _customizer;

        public CustomCommandLine(ICommandLine baseCommandLine, Func<CommandLine, ICommandLine> customizer)
        {
            _baseCommandLine = baseCommandLine;
            _customizer = customizer;
        }

        public IStartInfo GetStartInfo(IHost host) =>
            _customizer(new CommandLine(_baseCommandLine.GetStartInfo(host))).GetStartInfo(host);

        public void PreRun(IHost host) => _baseCommandLine.PreRun(host);

        public IEnumerable<IServiceMessage> GetNonStdStreamsServiceMessages(IHost host) => _baseCommandLine.GetNonStdStreamsServiceMessages(host);
    }
}
