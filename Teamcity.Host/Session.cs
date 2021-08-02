// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.Host
{
    using System;

    internal partial class Session : ISession
    {
        public string Id { get; set; } = "csi" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
    }
}