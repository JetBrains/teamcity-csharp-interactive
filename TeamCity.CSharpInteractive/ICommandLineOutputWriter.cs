namespace TeamCity.CSharpInteractive
{
    using Cmd;

    internal interface ICommandLineOutputWriter
    {
        void Write(CommandLineOutput output);
    }
}