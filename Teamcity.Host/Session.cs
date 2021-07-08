// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;

    internal class Session : ISession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }
}