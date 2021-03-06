// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class OptimizationLevelSettingDescription : ISettingDescription
{
    public bool IsVisible => false;

    public Type SettingType => typeof(OptimizationLevel);

    public string Key => "ol";

    public string Description => "Set an optimization level";
}