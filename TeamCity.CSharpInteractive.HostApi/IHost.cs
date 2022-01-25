// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace HostApi;

public interface IHost
{
    IHost Host { get; }
        
    IReadOnlyList<string> Args { get; }
        
    IProperties Props { get; }

    void WriteLine();
        
    void WriteLine<T>(T line, Color color = Color.Default);
        
    void Error(string? error, string? errorId = default);
        
    void Warning(string? warning);
        
    void Info(string? text);
        
    void Trace(string? trace, string? origin = default);
        
    T GetService<T>();
}