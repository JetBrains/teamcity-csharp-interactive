#r "nuget:IoC.Container, 1.3.6"
using IoC;

var container = Container.Create();

WriteLine($"Result: {container}");