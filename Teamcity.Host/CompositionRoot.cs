// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    internal class CompositionRoot
    {
        public readonly ISession Session;
        public readonly Flow.FlowClient Flow;
        public readonly Console.ConsoleClient Console;
        public readonly Log.LogClient Log;
        
        public CompositionRoot(
            ISession session,
            Flow.FlowClient flow,
            Console.ConsoleClient console,
            Log.LogClient log)
        {
            Session = session;
            Flow = flow;
            Console = console;
            Log = log;
        }
    }
}