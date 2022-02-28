// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using HostApi;

internal class AddNuGetReferenceCommandRunner : ICommandRunner
{
    private readonly ILog<AddNuGetReferenceCommandRunner> _log;
    private readonly INuGetReferenceResolver _nuGetReferenceResolver;
    private readonly IReferenceRegistry _referenceRegistry;

    public AddNuGetReferenceCommandRunner(
        ILog<AddNuGetReferenceCommandRunner> log,
        INuGetReferenceResolver nuGetReferenceResolver,
        IReferenceRegistry referenceRegistry)
    {
        _log = log;
        _nuGetReferenceResolver = nuGetReferenceResolver;
        _referenceRegistry = referenceRegistry;
    }

    public CommandResult TryRun(ICommand command)
    {
        if (command is not AddNuGetReferenceCommand addPackageReferenceCommand)
        {
            return new CommandResult(command, default);
        }

        if (!_nuGetReferenceResolver.TryResolveAssemblies(addPackageReferenceCommand.PackageId, addPackageReferenceCommand.VersionRange, out var assemblies))
        {
            return new CommandResult(command, false);
        }

        var success = true;
        foreach (var assembly in assemblies)
        {
            if (_referenceRegistry.TryRegisterAssembly(assembly.FilePath, out var description))
            {
                _log.Info(Text.Tab, new Text(assembly.Name, Color.Highlighted));
            }
            else
            {
                _log.Error(ErrorId.NuGet, $"Cannot add the reference \"{assembly.Name}\": {description}");
                success = false;
            }
        }

        return new CommandResult(command, success);
    }
}