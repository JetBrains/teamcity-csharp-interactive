// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity
{
    using System;

    [Immutype.Target]
    public record Endpoint(string AccessToken, Uri? RequestUri = default);
}