// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    [ExcludeFromCodeCoverage]
    internal class ExitManager: IActive
    {
        private readonly ISettings _settings;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ExitManager(
            ISettings settings,
            CancellationTokenSource cancellationTokenSource)
        {
            _settings = settings;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public IDisposable Activate()
        {
            if (_settings.InteractionMode != InteractionMode.Interactive)
            {
                return Disposable.Empty;
            }

            System.Console.TreatControlCAsInput = false;
            System.Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            return Disposable.Create(() => System.Console.CancelKeyPress -= ConsoleOnCancelKeyPress);
        }

        private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            _cancellationTokenSource.Dispose();
            System.Environment.Exit(0);
        }
    }
}