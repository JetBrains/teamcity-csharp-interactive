namespace Teamcity.Host
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            .Default(Singleton)
            .Bind<IHostEnvironment>().To<HostEnvironment>()
            .Bind<ITeamCitySettings>().To<TeamCitySettings>()
            .Bind<IColorTheme>().To<ColorTheme>()
            .Bind<ITeamCityLineFormatter>().To<TeamCityLineFormatter>()
            .Bind<IStdOut>().Bind<IStdErr>().Tag("Default").To<ConsoleOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().Tag("TeamCity").To<TeamCityOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<IStdOut>("TeamCity") : ctx.Resolve<IStdOut>("Default"))

            // Service messages
            .Bind<ITeamCityBlockWriter<IDisposable>>().Bind<ITeamCityMessageWriter>().Bind<ITeamCityBuildProblemWriter>().To<HierarchicalTeamCityWriter>()
            .Bind<ITeamCityServiceMessages>().To<TeamCityServiceMessages>()
            .Bind<IServiceMessageFormatter>().To<ServiceMessageFormatter>()
            .Bind<IFlowIdGenerator>().To<FlowIdGenerator>()
            .Bind<DateTime>().As(Transient).To(_ => DateTime.Now)
            .Bind<IServiceMessageUpdater>().To<TimestampUpdater>()
            .Bind<ITeamCityWriter>().Tag("Root").To(
                ctx => ctx.Resolve<ITeamCityServiceMessages>().CreateWriter(
                    str => ((IStdOut) ctx.Resolve<IStdOut>("Default")).WriteLine(new Text(str + "\n"))))
            .Bind<IServiceMessageParser>().To<ServiceMessageParser>();
    }
}