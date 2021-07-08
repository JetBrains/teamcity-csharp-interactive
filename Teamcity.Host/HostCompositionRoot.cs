// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;

    internal class HostCompositionRoot
    {
        public readonly ISession Session;
        public readonly IObserver<DtoSession> SessionObserver;
        public readonly IObserver<DtoStdOut> StdOutObserver;
        public readonly IObserver<DtoError> ErrorObserver;
        public readonly IObserver<DtoWarning> WarningObserver;
        public readonly IObserver<DtoInfo> InfoObserver;
        public readonly IObserver<DtoTrace> TraceObserver;

        public HostCompositionRoot(
            ISession session,
            IObserver<DtoSession> sessionObserver,
            IObserver<DtoStdOut> stdOutObserver,
            IObserver<DtoError> errorObserver,
            IObserver<DtoWarning> warningObserver,
            IObserver<DtoInfo> infoObserver,
            IObserver<DtoTrace> traceObserver)
        {
            Session = session;
            SessionObserver = sessionObserver;
            StdOutObserver = stdOutObserver;
            ErrorObserver = errorObserver;
            WarningObserver = warningObserver;
            InfoObserver = infoObserver;
            TraceObserver = traceObserver;
        }
    }
}