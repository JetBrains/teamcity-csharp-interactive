// ReSharper disable CheckNamespace
namespace TeamCity.BuildApi
{
    using System;

    public class GetAllBuilds: IRestRequest<BuildsDto>
    {
        public Uri RelativeUri => new("builds", UriKind.Relative);
    }
}