// ReSharper disable CheckNamespace
namespace Cmd
{
    using System.Collections.Generic;

    public interface IStartInfo
    {
        string ShortName { get; }
        
        string ExecutablePath { get; }

        string WorkingDirectory { get; }

        IEnumerable<string> Args { get; }

        IEnumerable<(string name, string value)> Vars { get; }
    }
}