namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Cmd;

    internal class TestContext
    {
        public readonly string Name;
        public readonly List<Output> Output = new();

        public TestContext(string name) => Name = name;

        public void AddStdOut(IStartInfo info, string? text)
        {
            if (text != default)
            {
                Output.Add(new Output(info, false, text));
            }
        }

        public void AddStdErr(IStartInfo info, string? error)
        {
            if (error != default)
            {
                Output.Add(new Output(info, true, error));
            }
        }
    }
}