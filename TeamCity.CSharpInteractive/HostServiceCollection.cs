// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using Microsoft.Extensions.DependencyInjection;

internal class HostServiceCollection: ServiceCollection
{
    public HostServiceCollection() => this.AddComposer();
}