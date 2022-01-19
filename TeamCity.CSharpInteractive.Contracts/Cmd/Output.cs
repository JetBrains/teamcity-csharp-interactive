// ReSharper disable CheckNamespace
namespace Cmd
{
    [Immutype.Target]
    public readonly record struct Output(IStartInfo StartInfo, bool IsError, string Line, int ProcessId)
    {
        public static implicit operator string(Output it) => it.ToString();
        
        public override string ToString() => IsError ? $"ERR {Line}" : Line;
    }
}