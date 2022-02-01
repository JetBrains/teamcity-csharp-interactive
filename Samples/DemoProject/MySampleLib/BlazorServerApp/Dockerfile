FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 80
COPY . /app
WORKDIR /app
ENTRYPOINT ["dotnet", "BlazorServerApp.dll"]