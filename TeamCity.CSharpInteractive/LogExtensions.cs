// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal static class LogExtensions
{
    public static ILog<T> Error<T>(this ILog<T> log, ErrorId id, params string[] error)
    {
        log.Error(id, error.Select(i => new Text(i)).ToArray());
        return log;
    }
        
    public static ILog<T> Error<T>(this ILog<T> log, ErrorId id, Exception error)
    {
        log.Error(id, new Text(error.Message), Text.NewLine, new Text(error.StackTrace ?? "Empty stack trace."));
        return log;
    }
        
    public static ILog<T> Info<T>(this ILog<T> log, params string[] message)
    {
        log.Info(message.Select(i => new Text(i)).ToArray());
        return log;
    }
        
    public static ILog<T> Warning<T>(this ILog<T> log, params string[] warning)
    {
        log.Warning(warning.Select(i => new Text(i)).ToArray());
        return log;
    }
}