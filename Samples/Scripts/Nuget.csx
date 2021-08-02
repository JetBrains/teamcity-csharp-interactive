#l trace
#r "nuget:IoC.Container, 1.3.4"
using IoC;
#load "abc.csx"
WriteLine($"Result: {container}");
