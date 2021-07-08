// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;
    using System.IO;
    using System.IO.Pipes;
    using Newtonsoft.Json;

    internal class PipeObserver<T>: IObserver<T>
    {
        private readonly ISession _session;

        public PipeObserver(ISession session) => _session = session;

        public void OnNext(T value)
        {
            try
            {
                using var stream = CreateStream();
                var serialised = JsonConvert.SerializeObject(value);
                using var writer = new StreamWriter(stream);
                writer.WriteLine(serialised);
            }
            catch
            {
                // ignored
            }
        }
        
        public void OnError(Exception error) { }

        public void OnCompleted() { }

        private NamedPipeClientStream CreateStream()
        {
            NamedPipeClientStream namedPipeClient = new(_session.Id);
            namedPipeClient.Connect();
            return namedPipeClient;
        }
    }
}