// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Linq;

    internal static class LogExtensions
    {
        public static IDisposable Block<T>(this ILog<T> log, string blockName) => 
            string.IsNullOrWhiteSpace(blockName) ? Disposable.Empty : log.Block(new[] {new Text(blockName)});

        public static ILog<T> Error<T>(this ILog<T> log, params string[] error)
        {
            log.Error(error.Select(i => new Text(i)).ToArray());
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
        
        public static ILog<T> Trace<T>(this ILog<T> log, params string[] traceMessage)
        {
            log.Trace(traceMessage.Select(i => new Text(i)).ToArray());
            return log;
        }
    }
}