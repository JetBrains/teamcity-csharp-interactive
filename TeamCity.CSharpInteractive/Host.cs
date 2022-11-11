// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using HostApi;
using NuGet.Versioning;
using TeamCity.CSharpInteractive;

[ExcludeFromCodeCoverage]
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
#if APPLICATION
public static class Host
#else
public static class Components
#endif
{
    private static readonly ScriptHostComponents HostComponents = Composer.ResolveScriptHostComponents();
    private static readonly IHost CurHost = HostComponents.Host;

#if APPLICATION
    private static readonly IDisposable FinishToken;

    static Host()
    {
        if (AppDomain.CurrentDomain.GetAssemblies().Any(i => string.Equals("dotnet-csi", i.GetName().Name, StringComparison.InvariantCultureIgnoreCase)))
        {
            FinishToken = Disposable.Empty;
            return;
        }

        HostComponents.Info.ShowHeader();
        FinishToken = Disposable.Create(HostComponents.ExitTracker.Track(), HostComponents.Statistics.Start());
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Finish();
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }

    private static void Finish()
    {
        try
        {
            FinishToken.Dispose();
            Composer.FinalDispose();
        }
        catch (Exception ex)
        {
            try
            {
                HostComponents.Log.Error(ErrorId.Unhandled, ex.Message);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
    
    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            HostComponents.Log.Error(ErrorId.Exception, new[] {new Text(e.ExceptionObject.ToString() ?? "Unhandled exception.")});
            Finish();
            System.Environment.Exit(1);
        }
        catch
        {
            // ignored
        }
    }
#endif

    public static IReadOnlyList<string> Args => CurHost.Args;

    public static IProperties Props => CurHost.Props;

    public static void WriteLine() => CurHost.WriteLine();

    public static void WriteLine<T>(T line, Color color = Color.Default) => CurHost.WriteLine(line, color);

    public static void Error(string? error, string? errorId = default) => CurHost.Error(error, errorId);

    public static void Warning(string? warning) => CurHost.Warning(warning);

    public static void Info(string? text) => CurHost.Info(text);

    public static void Trace(string? trace, string? origin = default) => CurHost.Trace(trace, origin);

    [Pure]
    public static T GetService<T>() => CurHost.GetService<T>();
    
    public static int? Run(this ICommandLine commandLine, Action<Output>? handler = default, TimeSpan timeout = default) => 
        HostComponents.CommandLineRunner.Run(commandLine, handler, timeout);

    public static Task<int?> RunAsync(this ICommandLine commandLine, Action<Output>? handler = default, CancellationToken cancellationToken = default) =>
        HostComponents.CommandLineRunner.RunAsync(commandLine, handler, cancellationToken);
    
    public static IBuildResult Build(this ICommandLine commandLine, Action<BuildMessage>? handler = default, TimeSpan timeout = default) => 
        HostComponents.BuildRunner.Run(commandLine, handler, timeout);

    public static Task<IBuildResult> BuildAsync(this ICommandLine commandLine, Action<BuildMessage>? handler = default, CancellationToken cancellationToken = default) =>
        HostComponents.BuildRunner.RunAsync(commandLine, handler, cancellationToken);
    
    [Obsolete]
    public static IEnumerable<NuGetPackage> Restore(this INuGet nuGet, string packageId, string? versionRange = default, string? targetFrameworkMoniker = default, string? packagesPath = default) =>
        nuGet.Restore(
            new NuGetRestoreSettings(packageId)
                .WithVersionRange(versionRange != default ? VersionRange.Parse(versionRange) : default)
                .WithTargetFrameworkMoniker(targetFrameworkMoniker)
                .WithPackagesPath(packagesPath));

    public static bool TryGetValue<T>(this IProperties properties, string key, [MaybeNullWhen(false)] out T value)
    {
        if (properties.TryGetValue(key, out var valStr))
        {
            if (typeof(T) == typeof(string))
            {
                value = (T)(object)valStr;
                return true;
            }
            
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(typeof(string)))
            {
                var nullableValue = converter.ConvertFrom(valStr);
                if (!Equals(nullableValue, null))
                {
                    try
                    {
                        value = (T)nullableValue;
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                    }
                }
            }
        }

        value = default;
        return false;
    }
}