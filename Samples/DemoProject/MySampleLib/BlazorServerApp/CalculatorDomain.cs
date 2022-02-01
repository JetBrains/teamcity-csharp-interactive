// ReSharper disable UnusedType.Global
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Local
namespace BlazorServerApp
{
    using MySampleLib;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    internal static partial class CalculatorDomain
    {
        private static void Setup() => DI.Setup()
            .Bind<ICalculator>().As(ContainerSingleton).To<Calculator>();
    }
}