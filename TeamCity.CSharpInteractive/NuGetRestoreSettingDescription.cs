// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class NuGetRestoreSettingDescription : ISettingDescription
{
    public bool IsVisible => false;

    public Type SettingType => typeof(NuGetRestoreSetting);

    public string Key => "nr";

    public string Description => "Set a NuGet restore setting";
}