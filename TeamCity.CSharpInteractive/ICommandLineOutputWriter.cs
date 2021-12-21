namespace TeamCity.CSharpInteractive
{
    using Cmd;

    internal interface ICommandLineOutputWriter
    {
        void Write(in CommandLineOutput output);
    }
}