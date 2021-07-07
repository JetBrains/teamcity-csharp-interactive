// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCast
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Host;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
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
            
            // Host
            .Bind<IHostEnvironment>().To(_ => Teamcity.Host.Composer.Resolve<IHostEnvironment>())
            .Bind<ITeamCitySettings>().To(_ => Teamcity.Host.Composer.Resolve<ITeamCitySettings>())
            .Bind<ITeamCityLineFormatter>().To(_ => Teamcity.Host.Composer.Resolve<ITeamCityLineFormatter>())
            .Bind<ITeamCityBlockWriter<IDisposable>>().To(_ => Teamcity.Host.Composer.Resolve<ITeamCityBlockWriter<IDisposable>>())
            .Bind<ITeamCityMessageWriter>().To(_ => Teamcity.Host.Composer.Resolve<ITeamCityMessageWriter>())
            .Bind<ITeamCityBuildProblemWriter>().To(_ => Teamcity.Host.Composer.Resolve<ITeamCityBuildProblemWriter>())
            .Bind<IStdOut>().To(_ => Teamcity.Host.Composer.Resolve<IStdOut>())
            .Bind<IStdErr>().To(_ => Teamcity.Host.Composer.Resolve<IStdErr>());
    }
}