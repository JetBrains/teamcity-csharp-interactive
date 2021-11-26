// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    internal class WellknownValueResolver : IWellknownValueResolver
    {
        public string Resolve(string value) => value;
    }
}