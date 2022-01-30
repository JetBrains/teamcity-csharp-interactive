// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

internal class ExitCodeParser : IExitCodeParser
{
    public bool TryParse(object returnValue, out int exitCode)
    {
        unchecked
        {
            switch (returnValue)
            {
                case int intExitCode:
                    exitCode = intExitCode;
                    return true;

                case uint uintExitCode:
                    exitCode = (int)uintExitCode;
                    return true;

                case short shortExitCode:
                    exitCode = shortExitCode;
                    return true;

                case ushort ushortExitCode:
                    exitCode = ushortExitCode;
                    return true;

                case sbyte sbyteExitCode:
                    exitCode = sbyteExitCode;
                    return true;

                case byte byteExitCode:
                    exitCode = byteExitCode;
                    return true;

                case long longExitCode:
                    exitCode = (int)longExitCode;
                    return true;

                case ulong ulongExitCode:
                    exitCode = (int)ulongExitCode;
                    return true;

                default:
                    exitCode = default;
                    return false;
            }
        }
    }
}