namespace TeamCity.CSharpInteractive
{
    using Dotnet;

    internal interface IBuildMessageLogWriter
    {
        void Write(BuildMessage message);
    }
}