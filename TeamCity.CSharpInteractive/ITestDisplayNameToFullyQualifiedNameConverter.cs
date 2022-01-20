namespace TeamCity.CSharpInteractive;

internal interface ITestDisplayNameToFullyQualifiedNameConverter
{
    string Convert(string displayName);
}