dotnet clean SelfBuild\SelfBuild.csproj
dotnet pack -c Release TeamCity.CSharpInteractive/TeamCity.CSharpInteractive.csproj
rmdir "%USERPROFILE%\.nuget\packages\teamcity.csharpinteractive" /S /Q
dotnet restore SelfBuild\SelfBuild.csproj --source "%~dp0TeamCity.CSharpInteractive\bin\Release"
dotnet run --no-restore --project SelfBuild -- -p integrationTests=false