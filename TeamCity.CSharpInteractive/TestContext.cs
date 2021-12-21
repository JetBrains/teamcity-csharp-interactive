namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;
using Cmd;

internal class TestContext
{
    public readonly string Name;
    public readonly List<CommandLineOutput> Output = new();

    public TestContext(string name) => Name = name;

    public void AddStdOut(in CommandLine commandLine, string? text)
    {
        if (text != default)
        {
            Output.Add(new CommandLineOutput(commandLine, false, text));
        }
    }

    public void AddStdErr(in CommandLine commandLine, string? error)
    {
        if (error != default)
        {
            Output.Add(new CommandLineOutput(commandLine, true, error));
        }
    }
}