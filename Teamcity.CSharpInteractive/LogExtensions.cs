// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [ExcludeFromCodeCoverage]
    internal static class LogExtensions
    {
        public static IDisposable Block<T>(this ILog<T> log, string blockName) => 
            string.IsNullOrWhiteSpace(blockName) ? Disposable.Empty : log.Block(new[] {new Text(blockName)});

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
        
        public static ILog<T> Trace<T>(this ILog<T> log, params Text[] traceMessage)
        {
            log.Trace(string.Empty, traceMessage);
            return log;
        }
        
        public static ILog<T> Trace<T>(this ILog<T> log, string origin, params string[] traceMessage)
        {
            log.Trace(origin, traceMessage.Select(i => new Text(i)).ToArray());
            return log;
        }
        
        public static ILog<T> Trace<T>(this ILog<T> log, params string[] traceMessage)
        {
            log.Trace(string.Empty, traceMessage);
            return log;
        }
    }
}