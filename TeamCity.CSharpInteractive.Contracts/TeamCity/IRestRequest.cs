// ReSharper disable CheckNamespace
namespace TeamCity
{
    using System;

    public interface IRestRequest<TResponse>
    {
        Uri RelativeUri { get; }
    }
}