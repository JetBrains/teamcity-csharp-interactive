namespace TeamCity.CSharpInteractive;

internal interface ICISpecific<out T>
{
    T Instance { get; }
}