namespace TeamCity.CSharpInteractive;

using System.Collections.Generic;

public interface IJavaPropertiesParser
{
    IReadOnlyDictionary<string, string> Parse(IEnumerable<string> lines);
}