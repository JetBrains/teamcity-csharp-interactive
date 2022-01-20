namespace TeamCity.CSharpInteractive;

internal interface ITraceSource
{
    IEnumerable<Text> Trace { get; }
}