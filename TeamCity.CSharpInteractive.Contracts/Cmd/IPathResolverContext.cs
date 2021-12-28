// ReSharper disable CheckNamespace
namespace Cmd
{
    using System;

    internal interface IPathResolverContext
    {
        IDisposable Register(IPathResolver resolver);
    }
}