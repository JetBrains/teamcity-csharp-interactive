namespace TeamCity.CSharpInteractive.Contracts
{
    internal interface IWellknownValueResolver
    {
        string Resolve(WellknownValue value);
    }
}