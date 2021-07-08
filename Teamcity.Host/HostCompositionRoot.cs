// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;

    internal class HostCompositionRoot
    {
        public readonly ISession Session;
        public readonly IObserver<SessionContent> SessionObserver;
        public readonly IObserver<StdOutContent> StdOutObserver;
        public readonly IObserver<ErrorContent> ErrorObserver;
        public readonly IObserver<WarningContent> WarningObserver;
        public readonly IObserver<InfoContent> InfoObserver;
        public readonly IObserver<TraceContent> TraceObserver;

        public HostCompositionRoot(
            ISession session,
            IObserver<SessionContent> sessionObserver,
            IObserver<StdOutContent> stdOutObserver,
            IObserver<ErrorContent> errorObserver,
            IObserver<WarningContent> warningObserver,
            IObserver<InfoContent> infoObserver,
            IObserver<TraceContent> traceObserver)
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