// ReSharper disable CheckNamespace
namespace Cmd
{
    public readonly record struct Output(IStartInfo StartInfo, bool IsError, string Line, int ProcessId)
    {
        public override string ToString() => !IsError ? Line : $"Error \"{Line}\"";
    }
}