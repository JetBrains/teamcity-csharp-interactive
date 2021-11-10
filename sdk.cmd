set SDKVersion=6.0
set WorkingDirectory=wd

docker pull mcr.microsoft.com/dotnet/sdk:%SDKVersion%
@for /f %%i in ('docker system info --format "{{.OSType}}"') do set OSType=%%i
@SET DRIVE=/
@IF [%OSType%]==[windows] SET DRIVE=C:/
docker run -it --rm "-w=%DRIVE%%WorkingDirectory%" "--volume=%~dp0:%DRIVE%%WorkingDirectory%" mcr.microsoft.com/dotnet/sdk:%SDKVersion% %*