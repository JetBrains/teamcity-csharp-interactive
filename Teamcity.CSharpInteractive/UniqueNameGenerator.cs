// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;

    internal class UniqueNameGenerator : IUniqueNameGenerator
    {
        public string Generate() => Guid.NewGuid().ToString().Replace("-", string.Empty);
    }
}