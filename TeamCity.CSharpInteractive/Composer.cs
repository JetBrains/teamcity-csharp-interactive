// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCast
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Threading;
    using Contracts;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using JetBrains.TeamCity.ServiceMessages.Write;
    using JetBrains.TeamCity.ServiceMessages.Write.Special;
    using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Updater;
    using Microsoft.Build.Framework;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Scripting;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        private static readonly Version ToolVersion = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version();

        static Composer()
        {
            DI.Setup()
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
                .Bind<IEnvironment>().Bind<IScriptContext>().To<Environment>()
                .Bind<ITeamCitySettings>().To<TeamCitySettings>()
                .Bind<IExitTracker>().To<ExitTracker>()
                .Bind<ITraceSource>().Tag(typeof(IEnvironment)).As(Transient).To(ctx => ctx.Resolve<IEnvironment>())
                .Bind<IDotnetEnvironment>().To<DotnetEnvironment>()
                .Bind<ITraceSource>().Tag(typeof(IDotnetEnvironment)).As(Transient).To(ctx => ctx.Resolve<IDotnetEnvironment>())
                .Bind<INugetEnvironment>().To<NugetEnvironment>()
                .Bind<ITraceSource>().Tag(typeof(INugetEnvironment)).As(Transient).To(ctx => ctx.Resolve<INugetEnvironment>())
                .Bind<ISettings>().Bind<ISettingsManager>().Bind<ISettingSetter<VerbosityLevel>>().Bind<Settings>().To<Settings>()
                .Bind<ISettingDescription>().Tag(typeof(VerbosityLevel)).To<VerbosityLevelSettingDescription>()
                .Bind<ICommandLineParser>().To<CommandLineParser>()
                .Bind<IInfo>().To<Info>()
                .Bind<ICodeSource>().To<ConsoleSource>()
                .Bind<ICodeSource>().Tag("Host").To<HostIntegrationCodeSource>()
                .Bind<FileCodeSource>().To<FileCodeSource>()
                .Bind<IFileCodeSourceFactory>().To<FileCodeSourceFactory>()
                .Bind<IRunner>().Tag(InteractionMode.Interactive).To<InteractiveRunner>()
                .Bind<IRunner>().Tag(InteractionMode.Script).To<ScriptRunner>()
                .Bind<IRunner>().As(Transient).To(ctx => ctx.Resolve<ISettings>().InteractionMode == InteractionMode.Interactive ? ctx.Resolve<IRunner>(InteractionMode.Interactive) : ctx.Resolve<IRunner>(InteractionMode.Script))
                .Bind<ICommandSource>().To<CommandSource>()
                .Bind<IStringService>().To<StringService>()
                .Bind<IStatistics>().To<Statistics>()
                .Bind<IFileTextReader>().To<FileTextReader>()
                .Bind<IPresenter<IEnumerable<ITraceSource>>>().To<TracePresenter>()
                .Bind<IPresenter<IStatistics>>().To<StatisticsPresenter>()
                .Bind<IPresenter<CompilationDiagnostics>>().To<DiagnosticsPresenter>()
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
                .Bind<IProperties>().Tag("Default").To<Properties>()
                .Bind<IProperties>().Tag("TeamCity").To<TeamCityProperties>()
                .Bind<IProperties>().To(ctx => ctx.Resolve<ITeamCitySettings>().IsUnderTeamCity ? ctx.Resolve<IProperties>("TeamCity") : ctx.Resolve<IProperties>("Default"))

                // Script options factory
                .Bind<IScriptOptionsFactory>()
                    .Bind<IReferenceRegistry>()
                    .Bind<ISettingSetter<LanguageVersion>>()
                    .Bind<ISettingSetter<OptimizationLevel>>()
                    .Bind<ISettingSetter<WarningLevel>>()
                    .Bind<ISettingSetter<CheckOverflow>>()
                    .Bind<ISettingSetter<AllowUnsafe>>()
                    .To<ScriptOptionsFactory>()

                .Bind<ICommandFactory<string>>().Tag("REPL Set a C# language version parser").To<SettingCommandFactory<LanguageVersion>>()
                .Bind<ICommandRunner>().Tag("REPL Set a C# language version").To<SettingCommandRunner<LanguageVersion>>()
                .Bind<ISettingDescription>().Tag(typeof(LanguageVersion)).To<LanguageVersionSettingDescription>()

                .Bind<ICommandFactory<string>>().Tag("REPL Set an optimization level parser").To<SettingCommandFactory<OptimizationLevel>>()
                .Bind<ICommandRunner>().Tag("REPL Set an optimization level").To<SettingCommandRunner<OptimizationLevel>>()
                .Bind<ISettingDescription>().Tag(typeof(OptimizationLevel)).To<OptimizationLevelSettingDescription>()

                .Bind<ICommandFactory<string>>().Tag("REPL Set a warning level parser").To<SettingCommandFactory<WarningLevel>>()
                .Bind<ICommandRunner>().Tag("REPL Set a warning level").To<SettingCommandRunner<WarningLevel>>()
                .Bind<ISettingDescription>().Tag(typeof(WarningLevel)).To<WarningLevelSettingDescription>()

                .Bind<ICommandFactory<string>>().Tag("REPL Set an overflow check parser").To<SettingCommandFactory<CheckOverflow>>()
                .Bind<ICommandRunner>().Tag("REPL Set an overflow check").To<SettingCommandRunner<CheckOverflow>>()
                .Bind<ISettingDescription>().Tag(typeof(CheckOverflow)).To<CheckOverflowSettingDescription>()
                
                .Bind<ICommandFactory<string>>().Tag("REPL Set allow unsafe parser").To<SettingCommandFactory<AllowUnsafe>>()
                .Bind<ICommandRunner>().Tag("REPL Set allow unsafe").To<SettingCommandRunner<AllowUnsafe>>()
                .Bind<ISettingDescription>().Tag(typeof(AllowUnsafe)).To<AllowUnsafeSettingDescription>()

                .Bind<IScriptSubmissionAnalyzer>().To<ScriptSubmissionAnalyzer>()
                .Bind<ICommandRunner>().Tag("CSharp").To<CSharpScriptCommandRunner>()
                .Bind<ICommandFactory<string>>().Tag("REPL Help parser").To<HelpCommandFactory>()
                .Bind<ICommandRunner>().Tag("REPL Help runner").To<HelpCommandRunner>()
                .Bind<ICommandFactory<string>>().Tag("REPL Set verbosity level parser").To<SettingCommandFactory<VerbosityLevel>>()
                .Bind<ICommandRunner>().Tag("REPL Set verbosity level runner").To<SettingCommandRunner<VerbosityLevel>>()
                .Bind<ICommandFactory<string>>().Tag("REPL Add NuGet reference parser").To<AddNuGetReferenceCommandFactory>()
                .Bind<IFilePathResolver>().To<FilePathResolver>()
                .Bind<ICommandFactory<string>>().Tag("REPL Add assembly reference parser").To<AddAssemblyReferenceCommandFactory>()
                .Bind<ICommandRunner>().Tag("REPL Add package reference runner").To<AddNuGetReferenceCommandRunner>()
                .Bind<ICommandFactory<string>>().Tag("REPL Load script").To<LoadCommandFactory>()

                // Service messages
                .Bind<ITeamCityWriter>()
                    .Bind<ITeamCityBlockWriter<ITeamCityWriter>>()
                    .Bind<ITeamCityFlowWriter<ITeamCityWriter>>()
                    .Bind<ITeamCityMessageWriter>()
                    .Bind<ITeamCityTestsWriter>()
                    .Bind<ITeamCityCompilationBlockWriter<ITeamCityWriter>>()
                    .Bind<ITeamCityArtifactsWriter>()
                    .Bind<ITeamCityBuildStatusWriter>()
                    .To<HierarchicalTeamCityWriter>()
                .Bind<ITeamCityServiceMessages>().To<TeamCityServiceMessages>()
                .Bind<IServiceMessageFormatter>().To<ServiceMessageFormatter>()
                .Bind<IFlowIdGenerator>().To<FlowIdGenerator>()
                .Bind<DateTime>().As(Transient).To(_ => DateTime.Now)
                .Bind<IServiceMessageUpdater>().To<TimestampUpdater>()
                .Bind<ITeamCityWriter>().Tag("Root").To(
                    ctx => ctx.Resolve<ITeamCityServiceMessages>().CreateWriter(
                        str => ((IStdOut)ctx.Resolve<IStdOut>("Default")).WriteLine(new Text(str + "\n"))))
                .Bind<IServiceMessageParser>().To<ServiceMessageParser>()
                
                // Public
                .Bind<IHost>().Bind<IServiceProvider>().To<HostService>()
                .Bind<INuGet>().To<NuGetService>();
        }
    }
}