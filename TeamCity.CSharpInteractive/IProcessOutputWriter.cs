namespace TeamCity.CSharpInteractive
{
    using Cmd;

    internal interface IProcessOutputWriter
    {
        void Write(in Output output);
    }
}