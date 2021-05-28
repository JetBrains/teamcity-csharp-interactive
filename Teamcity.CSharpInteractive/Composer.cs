// ReSharper disable PartialTypeWithSinglePart
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Scripting;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        static Composer() => DI.Setup()
                .Bind<Program>().As(Singleton).To<Program>()
                .Bind<ILog<TT>>().As(Singleton).To<Log<TT>>()
                .Bind<IEnvironment>().As(Singleton).To<Environment>()
                .Bind<ISettings>().As(Singleton).To<Settings>()
                .Bind<IInfo>().As(Singleton).To<Info>()
                .Bind<IColorTheme>().As(Singleton).To<ColorTheme>()
                .Bind<IStdOut>().Bind<IStdErr>().As(Singleton).To<ConsoleOutput>()
                .Bind<ICodeSource>().As(Singleton).To<ConsoleInput>()
                .Bind<FileCodeSource>().As(Singleton).To<FileCodeSource>()
                .Bind<IFileCodeSourceFactory>().As(Singleton).To<FileCodeSourceFactory>()
                .Bind<IRunner>().As(Singleton).Tag(InteractionMode.Interactive).To<InteractiveRunner>()
                .Bind<IRunner>().As(Singleton).Tag(InteractionMode.Script).To<ScriptRunner>()
                .Bind<ICommandSource>().As(Singleton).To<CommandSource>()
                .Bind<IStringService>().As(Singleton).To<StringService>()
                .Bind<IStatistics>().As(Singleton).To<Statistics>()
                .Bind<IFileTextReader>().As(Singleton).To<FileTextReader>()
                .Bind<IPresenter<IStatistics>>().As(Singleton).To<StatisticsPresenter>()
                .Bind<IPresenter<IEnumerable<Diagnostic>>>().As(Singleton).To<DiagnosticsPresenter>()
                .Bind<IPresenter<ScriptState<object>>>().As(Singleton).To<ScriptStatePresenter>()
            
                .Bind<IScriptCommandParser>().To<ScriptCommandParser>()
                .Bind<IScriptSubmissionAnalyzer>().As(Singleton).To<ScriptSubmissionAnalyzer>()
                .Bind<ICommandRunner>().As(Singleton).Tag("CSharpScript runner").To<CSharpScriptCommandRunner>()

                .Bind<IReplCommandParser>().As(Singleton).Tag("REPL Help parser").To<HelpCommandParser>()
                .Bind<ICommandRunner>().As(Singleton).Tag("REPL Help runner").To<HelpCommandRunner>()

                .Bind<IReplCommandParser>().As(Singleton).Tag("REPL Add reference parser").To<AddReferenceCommandParser>()
                .Bind<IReplCommandParser>().As(Singleton).Tag("REPL Load script parser").To<LoadScriptCommandParser>()

                .Bind<IReplCommandParser>().As(Singleton).Tag("REPL Set verbosity level parser").To<SetVerbosityLevelCommandParser>()
                .Bind<ICommandRunner>().As(Singleton).Tag("REPL Set verbosity level runner").To<SetVerbosityLevelCommandRunner>();
    }
}