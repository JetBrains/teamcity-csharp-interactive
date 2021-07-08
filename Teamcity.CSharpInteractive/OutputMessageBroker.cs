namespace Teamcity.CSharpInteractive
{
    using System;
    using Host;

    internal class OutputMessageBroker: IActive, IObserver<DtoStdOut>
    {
        private readonly IObservable<DtoStdOut> _outputSource;
        private readonly IStdOut _stdOut;

        public OutputMessageBroker(
            IObservable<DtoStdOut> outputSource,
            IStdOut stdOut)
        {
            _outputSource = outputSource;
            _stdOut = stdOut;
        }

        public IDisposable Activate() =>
        Disposable.Create(_outputSource.Subscribe(this));

        void IObserver<DtoStdOut>.OnNext(DtoStdOut value) =>
            _stdOut.WriteLine(new []{new Text(value.Line, value.Color)});

        void IObserver<DtoStdOut>.OnError(Exception error) { }

        void IObserver<DtoStdOut>.OnCompleted() { }
    }
}