dotnet clean Build\Build.csproj
dotnet pack -c Release TeamCity.CSharpInteractive/TeamCity.CSharpInteractive.csproj
rmdir "%USERPROFILE%\.nuget\packages\teamcity.csharpinteractive" /S /Q
dotnet restore Build\Build.csproj --source "%~dp0TeamCity.CSharpInteractive\bin\Release"
dotnet run --no-restore --project Build -- -p integrationTests=false