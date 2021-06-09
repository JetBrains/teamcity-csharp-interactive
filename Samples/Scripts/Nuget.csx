#l trace
#r IoC.Container 1.3.4
#r NuGet.Build.Tasks 5.9.1
using IoC;
var container = Container.Create();
Console.WriteLine(container);
