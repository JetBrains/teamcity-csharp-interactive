// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    internal class CompositionRoot
    {
        public readonly ISession Session;
        public readonly FlowService.FlowServiceClient Flow;
        public readonly ConsoleService.ConsoleServiceClient Console;
        public readonly LogService.LogServiceClient Log;
        
        public CompositionRoot(
            ISession session,
            FlowService.FlowServiceClient flow,
            ConsoleService.ConsoleServiceClient console,
            LogService.LogServiceClient log)
        {
            Session = session;
            Flow = flow;
            Console = console;
            Log = log;
        }
    }
}