// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Loader;
    using Contracts;

    [ExcludeFromCodeCoverage]
    public static class Host
    {
        private static readonly IHost BaseHost = Composer.ResolveIHost();
        private static readonly IStatistics Statistics = Composer.ResolveIStatistics();
        private static readonly IPresenter<IStatistics> StatisticsPresenter = Composer.ResolveIPresenterIStatistics();
        private static readonly ILog<HostService> Log = Composer.ResolveTeamCitySpecificILogHostService().Instance;

        static Host()
        {
            var statisticsToken = Statistics.Start();
            AssemblyLoadContext.Default.Unloading += _ =>
            {
                try
                {
                    statisticsToken.Dispose();
                    StatisticsPresenter.Show(Statistics);
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (Statistics.Errors.Any())
                    {
                        Log.Info(new Text("Running FAILED.", Color.Error));
                    }
                    else
                    {
                        Log.Info(new Text("Running succeeded.", Color.Success));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            };
        }

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