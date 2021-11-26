namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using Contracts;

    public static class Host
    {
        private static readonly IHost BaseHost = Composer.ResolveIHost();

        public static IReadOnlyList<string> Args => BaseHost.Args;

        public static IProperties Props => BaseHost.Props;

        public static void WriteLine() => BaseHost.WriteLine();

        public static void WriteLine<T>(T line, Color color = Color.Default) => BaseHost.WriteLine(line, color);

        public static void Error(string error, string errorId = "Unknown") => BaseHost.Error(error, errorId);

        public static void Warning(string warning) => BaseHost.Warning(warning);

        public static void Info(string text) => BaseHost.Info(text);

        public static void Trace(string trace, string origin = "") => BaseHost.Trace(trace, origin);

        public static T GetService<T>() => BaseHost.GetService<T>();
    }
}