// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCast
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Threading;
    using Host;
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
        private static readonly Version ToolVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();
        
        static Composer() => DI.Setup()
            .Default(Singleton)
            .Bind<Program>().To<Program>()
            .Bind<Version>().Tag("ToolVersion").To(_ => ToolVersion)
            .Bind<string>().Tag("TargetFrameworkMoniker").To(_ => Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? string.Empty)
            .Bind<CancellationTokenSource>().To(_ => new CancellationTokenSource())
            .Bind<IHostEnvironment>().To<HostEnvironment>()
            .Bind<ITeamCitySettings>().To<TeamCitySettings>()
            .Bind<IColorTheme>().To<ColorTheme>()
            .Bind<ITeamCityLineFormatter>().To<TeamCityLineFormatter>()
            .Bind<IStdOut>().Bind<IStdErr>().Tag("Default").To<ConsoleOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().Tag("TeamCity").To<TeamCityOutput>()
            .Bind<IStdOut>().Bind<IStdErr>().To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<IStdOut>("TeamCity") : ctx.Resolve<IStdOut>("Default"))
            .Bind<ILog<TT>>().Tag("Default").To<Log<TT>>()
            .Bind<ILog<TT>>().Tag("TeamCity").To<TeamCityLog<TT>>()
            .Bind<ILog<TT>>().To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<ILog<TT>>("TeamCity") : ctx.Resolve<ILog<TT>>("Default"))
            .Bind<IFileSystem>().To<FileSystem>()
            .Bind<IEnvironment>().Bind<IWorkingDirectoryContext>().To<Environment>()
            .Bind<ITeamCitySettings>().To<TeamCitySettings>()
            .Bind<IExitTracker>().To<ExitTracker>()
            .Bind<ITraceSource>().Tag(typeof(IEnvironment)).As(Transient).To(ctx => ctx.Resolve<IEnvironment>())
            .Bind<IDotnetEnvironment>().To<DotnetEnvironment>()
            .Bind<ITraceSource>().Tag(typeof(IDotnetEnvironment)).As(Transient).To(ctx => ctx.Resolve<IDotnetEnvironment>())
            .Bind<INugetEnvironment>().To<NugetEnvironment>()
            .Bind<ITraceSource>().Tag(typeof(INugetEnvironment)).As(Transient).To(ctx => ctx.Resolve<INugetEnvironment>())
            .Bind<ISettings>().Bind<ISettingsManager>().To<Settings>()
            .Bind<ICommandLineParser>().To<CommandLineParser>()
            .Bind<IInfo>().To<Info>()
            .Bind<ICodeSource>().To<ConsoleInput>()
            .Bind<ICodeSource>().Tag("Host").To<HostIntegrationCodeSource>()
            .Bind<FileCodeSource>().To<FileCodeSource>()
            .Bind<IFileCodeSourceFactory>().To<FileCodeSourceFactory>()
            .Bind<InitialStateCodeSource>().To<InitialStateCodeSource>()
            .Bind<IInitialStateCodeSourceFactory>().To<InitialStateCodeSourceFactory>()
            .Bind<IRunner>().Tag(InteractionMode.Interactive).To<InteractiveRunner>()
            .Bind<IRunner>().Tag(InteractionMode.Script).To<ScriptRunner>()
            .Bind<IRunner>().As(Transient).To(ctx => ctx.Resolve<ISettings>().InteractionMode == InteractionMode.Interactive ? ctx.Resolve<IRunner>(InteractionMode.Interactive) : ctx.Resolve<IRunner>(InteractionMode.Script))
            .Bind<ICommandSource>().To<CommandSource>()
            .Bind<IStringService>().To<StringService>()
            .Bind<IStatistics>().To<Statistics>()
            .Bind<IFileTextReader>().To<FileTextReader>()
            .Bind<IPresenter<IEnumerable<ITraceSource>>>().To<TracePresenter>()
            .Bind<IPresenter<IStatistics>>().To<StatisticsPresenter>()
            .Bind<IPresenter<IEnumerable<Diagnostic>>>().To<DiagnosticsPresenter>()
            .Bind<IPresenter<ScriptState<object>>>().To<ScriptStatePresenter>()
            .Bind<IBuildEngine>().To<BuildEngine>()
            .Bind<INugetRestoreService>().To<NugetRestoreService>()
            .Bind<NuGet.Common.ILogger>().To<NugetLogger>()
            .Bind<IUniqueNameGenerator>().To<UniqueNameGenerator>()
            .Bind<INugetAssetsReader>().To<NugetAssetsReader>()
            .Bind<ICleaner>().To<Cleaner>()
            .Bind<ICommandsRunner>().To<CommandsRunner>()
            .Bind<ICommandFactory<ICodeSource>>().To<CodeSourceCommandFactory>()
            .Bind<ICommandFactory<ScriptCommand>>().As(Transient).To<ScriptCommandFactory>()
            .Bind<ICSharpScriptRunner>().To<CSharpScriptRunner>()
            .Bind<IScriptSubmissionAnalyzer>().To<ScriptSubmissionAnalyzer>()
            .Bind<ICommandRunner>().Tag("CSharp").To<CSharpScriptCommandRunner>()
            .Bind<ICommandFactory<string>>().Tag("REPL Help parser").To<HelpCommandFactory>()
            .Bind<ICommandRunner>().Tag("REPL Help runner").To<HelpCommandRunner>()
            .Bind<ICommandFactory<string>>().Tag("REPL Set verbosity level parser").To<SetVerbosityLevelCommandFactory>()
            .Bind<ICommandRunner>().Tag("REPL Set verbosity level runner").To<SetVerbosityLevelCommandRunner>()
            .Bind<ICommandFactory<string>>().Tag("REPL Add package reference parser").To<AddPackageReferenceCommandFactory>()
            .Bind<ICommandRunner>().Tag("REPL Add package reference runner").To<AddPackageReferenceCommandRunner>()
            .Bind<ICommandFactory<string>>().Tag("REPL Load script").To<LoadCommandFactory>()

            // Messages
            .Bind<ISession>().As(Transient).To(_ => Teamcity.Host.Composer.Resolve<ISession>())
            .Bind<IObservable<string>>().To<PipeObservable>()
            .Bind<IActive>().Tag("Pipe").To(ctx => ctx.Resolve<IObservable<string>>())

            .Bind<IObservable<SessionContent>>().To<MessageObservable<SessionContent>>()
            .Bind<IObservable<ErrorContent>>().To<MessageObservable<ErrorContent>>()
            .Bind<IObservable<WarningContent>>().To<MessageObservable<WarningContent>>()
            .Bind<IObservable<InfoContent>>().To<MessageObservable<InfoContent>>()
            .Bind<IObservable<TraceContent>>().To<MessageObservable<TraceContent>>()
            .Bind<IActive>().Tag(nameof(LogMessageBroker)).To<LogMessageBroker>()
            
            .Bind<IObservable<StdOutContent>>().To<MessageObservable<StdOutContent>>()
            .Bind<IActive>().Tag(nameof(OutputMessageBroker)).To<OutputMessageBroker>()

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