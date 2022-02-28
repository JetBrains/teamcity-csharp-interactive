// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
internal class HostServiceCollection : ServiceCollection
{
    public HostServiceCollection() => this.AddComposer();
}