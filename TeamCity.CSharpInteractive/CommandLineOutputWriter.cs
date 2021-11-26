// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using Cmd;

    internal class CommandLineOutputWriter : ICommandLineOutputWriter
    {
        private readonly IConsole _console;

        public CommandLineOutputWriter(IConsole console) => _console = console;

        public void Write(CommandLineOutput output)
        {
            if (output.IsError)
            {
                _console.WriteToErr(output.Line, System.Environment.NewLine);
            }
            else
            {
                _console.WriteToOut((default, output.Line), (default, System.Environment.NewLine));
            }
        }
    }
}