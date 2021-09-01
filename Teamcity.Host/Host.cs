// ReSharper disable UnusedMember.Global
namespace Teamcity.Host
{
    using Google.Protobuf.WellKnownTypes;
    
    public static class Host
    {
        private static readonly CompositionRoot Root = Composer.Resolve<CompositionRoot>();
        
        public static void ScriptInternal_SetPort(int port) => Root.Session.Port = port;

        public static void ScriptInternal_FinishCommand() =>
            Root.Flow.Completed(new Empty());
        
        public static void WriteLine() =>
            Root.Console.WriteLine(new WriteLineRequest { Line = string.Empty, Color = Color.Default});

        public static void WriteLine<T>(T line, Color color = Color.Default) =>
            Root.Console.WriteLine(new WriteLineRequest { Line = line?.ToString(), Color = color});

        public static void Error(string error, string errorId = "Unknown") =>
            Root.Log.Error(new ErrorRequest { ErrorId = errorId, Error = error });

        public static void Warning(string warning) =>
            Root.Log.Warning(new WarningRequest { Warning = warning });

        public static void Info(string text) =>
            Root.Log.Info(new InfoRequest { Text = text });

        public static void Trace(string trace) =>
            Root.Log.Trace(new TraceRequest { Trace = trace });
    }
}