// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ExitTracker : IExitTracker
    {
        private readonly ILog<ExitTracker> _log;
        private readonly ISettings _settings;
        private readonly ISettingSetter<VerbosityLevel> _settingSetter;
        private readonly IInfo _info;
        private readonly IEnvironment _environment;

        public ExitTracker(
            ILog<ExitTracker> log,
            ISettings settings,
            ISettingSetter<VerbosityLevel> settingSetter,
            IInfo info,
            IEnvironment environment)
        {
            _log = log;
            _settings = settings;
            _settingSetter = settingSetter;
            _info = info;
            _environment = environment;
        }

        public IDisposable Track()
        {
            switch (_settings.InteractionMode)
            {
                case InteractionMode.Interactive:
                    Console.CancelKeyPress += ConsoleOnCancelKeyPress;
                    return Disposable.Create(() => Console.CancelKeyPress -= ConsoleOnCancelKeyPress);

                default:
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
                    return Disposable.Create(() => AppDomain.CurrentDomain.ProcessExit -= CurrentDomainOnProcessExit);
            }
        }

        private void CurrentDomainOnProcessExit(object? sender, EventArgs e)
        {
            _log.Error(ErrorId.AbnormalProgramTermination, "Abnormal program termination.");
            _settingSetter.SetSetting(VerbosityLevel.Diagnostic);
            _info.ShowFooter();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e) => _environment.Exit(ExitCode.Success);
    }
}