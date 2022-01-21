namespace Script.Cmd;

[Immutype.Target]
public readonly record struct Output(IStartInfo StartInfo, bool IsError, string Line, int ProcessId)
{
    public override string ToString() => IsError ? $"ERR {Line}" : Line;
}