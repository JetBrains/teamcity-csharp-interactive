// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Cmd;

internal class ProcessOutputWriter : IProcessOutputWriter
{
    private readonly IConsole _console;

    public ProcessOutputWriter(IConsole console) => _console = console;

    public void Write(in Output output)
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