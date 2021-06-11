// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCast
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Versioning;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Scripting;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            .Bind<Program>().As(Singleton).To<Program>()
            .Bind<string>().As(Singleton).Tag("TargetFrameworkMoniker").To(_ => Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? string.Empty)
            .Bind<ILog<TT>>().As(Singleton).Tag("Default").To<Log<TT>>()
            .Bind<ILog<TT>>().As(Singleton).Tag("TeamCity").To<TeamCityLog<TT>>()
            .Bind<ILog<TT>>().As(Singleton).To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<ILog<TT>>("TeamCity") : ctx.Resolve<ILog<TT>>("Default"))
            .Bind<IFileSystem>().As(Singleton).To<FileSystem>()
            .Bind<IEnvironment>().As(Singleton).To<Environment>()
            .Bind<ITeamCitySettings>().As(Singleton).To<TeamCitySettings>()
            .Bind<IExitTracker>().As(Singleton).To<ExitTracker>()
            .Bind<ITraceSource>().Tag(typeof(IEnvironment)).To(ctx => ctx.Resolve<IEnvironment>())
            .Bind<IDotnetEnvironment>().As(Singleton).To<DotnetEnvironment>()
            .Bind<ITraceSource>().Tag(typeof(IDotnetEnvironment)).To(ctx => ctx.Resolve<IDotnetEnvironment>())
            .Bind<INugetEnvironment>().As(Singleton).To<NugetEnvironment>()
            .Bind<ITraceSource>().Tag(typeof(INugetEnvironment)).To(ctx => ctx.Resolve<INugetEnvironment>())
            .Bind<ISettings>().As(Singleton).To<Settings>()
            .Bind<IInfo>().As(Singleton).To<Info>()
            .Bind<IColorTheme>().As(Singleton).To<ColorTheme>()
            .Bind<ITeamCityLineFormatter>().As(Singleton).To<TeamCityLineFormatter>()
            .Bind<IStdOut>().Bind<IStdErr>().As(Singleton).Tag("Default").To<ConsoleOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().As(Singleton).Tag("TeamCity").To<TeamCityOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().As(Singleton).To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<IStdOut>("TeamCity") : ctx.Resolve<IStdOut>("Default"))
            .Bind<ICodeSource>().As(Singleton).To<ConsoleInput>()
            .Bind<FileCodeSource>().As(Singleton).To<FileCodeSource>()
            .Bind<IFileCodeSourceFactory>().As(Singleton).To<FileCodeSourceFactory>()
            .Bind<IRunner>().As(Singleton).Tag(InteractionMode.Interactive).To<InteractiveRunner>()
            .Bind<IRunner>().As(Singleton).Tag(InteractionMode.Script).To<ScriptRunner>()
            .Bind<ICommandSource>().As(Singleton).To<CommandSource>()
            .Bind<IStringService>().As(Singleton).To<StringService>()
            .Bind<IStatistics>().As(Singleton).To<Statistics>()
            .Bind<IFileTextReader>().As(Singleton).To<FileTextReader>()
            .Bind<IPresenter<IEnumerable<ITraceSource>>>().As(Singleton).To<TracePresenter>()
            .Bind<IPresenter<IStatistics>>().As(Singleton).To<StatisticsPresenter>()
            .Bind<IPresenter<IEnumerable<Diagnostic>>>().As(Singleton).To<DiagnosticsPresenter>()
            .Bind<IPresenter<ScriptState<object>>>().As(Singleton).To<ScriptStatePresenter>()
            .Bind<IBuildEngine>().As(Singleton).To<BuildEngine>()
            .Bind<INugetRestoreService>().As(Singleton).To<NugetRestoreService>()
            .Bind<NuGet.Common.ILogger>().As(Singleton).To<NugetLogger>()
            .Bind<IUniqueNameGenerator>().As(Singleton).To<UniqueNameGenerator>()
            .Bind<INugetAssetsReader>().As(Singleton).To<NugetAssetsReader>()
            .Bind<ICleaner>().As(Singleton).To<Cleaner>()
            .Bind<ICommandsRunner>().As(Singleton).To<CommandsRunner>()
            .Bind<ICommandFactory<ICodeSource>>().As(Singleton).To<CodeSourceCommandFactory>()
            .Bind<ICommandFactory<ScriptCommand>>().To<ScriptCommandFactory>()
            .Bind<ICSharpScriptRunner>().As(Singleton).To<CSharpScriptRunner>()
            .Bind<IScriptSubmissionAnalyzer>().As(Singleton).To<ScriptSubmissionAnalyzer>()
            .Bind<ICommandRunner>().As(Singleton).Tag("CSharp").To<CSharpScriptCommandRunner>()
            .Bind<ICommandFactory<string>>().As(Singleton).Tag("REPL Help parser").To<HelpCommandFactory>()
            .Bind<ICommandRunner>().As(Singleton).Tag("REPL Help runner").To<HelpCommandRunner>()
            .Bind<ICommandFactory<string>>().As(Singleton).Tag("REPL Set verbosity level parser").To<SetVerbosityLevelCommandFactory>()
            .Bind<ICommandRunner>().As(Singleton).Tag("REPL Set verbosity level runner").To<SetVerbosityLevelCommandRunner>()
            .Bind<ICommandFactory<string>>().As(Singleton).Tag("REPL Add package reference parser").To<AddPackageReferenceCommandFactory>()
            .Bind<ICommandRunner>().As(Singleton).Tag("REPL Add package reference runner").To<AddPackageReferenceCommandRunner>()
        
            // Service messages
            .Bind<ITeamCityBlockWriter<IDisposable>>().Bind<ITeamCityMessageWriter>().Bind<ITeamCityBuildProblemWriter>().As(Singleton).To<HierarchicalTeamCityWriter>()
            .Bind<ITeamCityServiceMessages>().As(Singleton).To<TeamCityServiceMessages>()
            .Bind<IServiceMessageFormatter>().As(Singleton).To<ServiceMessageFormatter>()
            .Bind<IFlowIdGenerator>().As(Singleton).To<FlowIdGenerator>()
            .Bind<DateTime>().As(Singleton).To(_ => DateTime.Now)
            .Bind<IServiceMessageUpdater>().As(Singleton).To<TimestampUpdater>()
            .Bind<ITeamCityWriter>().Tag("Root").As(Singleton).To(
                ctx => ctx.Resolve<ITeamCityServiceMessages>().CreateWriter(
                    str => ((IStdOut)ctx.Resolve<IStdOut>("Default")).WriteLine(new Text(str + "\n"))))
            .Bind<IServiceMessageParser>().As(Singleton).To<ServiceMessageParser>();
    }
}