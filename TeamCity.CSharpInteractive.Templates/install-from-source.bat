dotnet pack -c Release TeamCity.CSharpInteractive.Templates.csproj
dotnet new -u TeamCity.CSharpInteractive.Templates
dotnet new -i TeamCity.CSharpInteractive.Templates::1.0.0-dev --nuget-source "%~dp0.\bin\Release"