namespace Teamcity.Host
{
    public static class Host
    {
        public static void WriteLine<T>(T line, Color color = Color.Default) =>
            Composer.Resolve<IStdOut>().WriteLine(new Text(line?.ToString() ?? string.Empty, color));
        
        public static void WriteError<T>(T error) =>
            Composer.Resolve<IStdErr>().WriteLine(new Text(error?.ToString() ?? string.Empty));
    }
}