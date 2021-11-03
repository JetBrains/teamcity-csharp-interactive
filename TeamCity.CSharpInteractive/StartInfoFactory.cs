// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;

    internal class StartInfoFactory : IStartInfoFactory
    {
        public ProcessStartInfo Create(Contracts.CommandLine commandLine)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = commandLine.ExecutablePath,
                WorkingDirectory = commandLine.WorkingDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var arg in commandLine.Args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            foreach (var (name, value) in commandLine.Vars)
            {
                startInfo.Environment[name] = value;
            }
            
            return startInfo;
        }
    }
}