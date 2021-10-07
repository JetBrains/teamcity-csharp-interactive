// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Threading;

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

            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            return Disposable.Create(() => Console.CancelKeyPress -= ConsoleOnCancelKeyPress);
        }

        private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            _cancellationTokenSource.Dispose();
            System.Environment.Exit(0);
        }
    }
}