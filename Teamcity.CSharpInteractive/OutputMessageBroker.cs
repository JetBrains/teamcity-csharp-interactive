namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal class OutputMessageBroker: IActive, IObserver<StdOutContent>
    {
        private readonly IObservable<StdOutContent> _outputSource;
        private readonly IStdOut _stdOut;

        public OutputMessageBroker(
            IObservable<StdOutContent> outputSource,
            IStdOut stdOut)
        {
            _outputSource = outputSource;
            _stdOut = stdOut;
        }

        public IDisposable Activate() =>
        Disposable.Create(_outputSource.Subscribe(this));

        void IObserver<StdOutContent>.OnNext(StdOutContent value) =>
            _stdOut.WriteLine(new []{new Text(value.Line, value.Color)});

        void IObserver<StdOutContent>.OnError(Exception error) { }

        void IObserver<StdOutContent>.OnCompleted() { }
    }
}