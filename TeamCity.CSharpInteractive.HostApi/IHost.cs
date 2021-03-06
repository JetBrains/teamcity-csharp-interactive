// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace HostApi;

using System.Diagnostics.Contracts;

public interface IHost
{
    IReadOnlyList<string> Args { get; }

    IProperties Props { get; }

    void WriteLine();

    void WriteLine<T>(T line, Color color = Color.Default);

    void Error(string? error, string? errorId = default);

    void Warning(string? warning);

    void Info(string? text);

    void Trace(string? trace, string? origin = default);

    [Pure]
    T GetService<T>();
}