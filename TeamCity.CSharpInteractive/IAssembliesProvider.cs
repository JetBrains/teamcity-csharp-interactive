namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using System.Reflection;

internal interface IAssembliesProvider
{
    IEnumerable<Assembly> GetAssemblies(IEnumerable<Type> types);
}