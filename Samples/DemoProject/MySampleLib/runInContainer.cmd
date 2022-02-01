rem docker build -f BlazorServerApp\Dockerfile -t BlazorServerApp "BlazorServerApp/bin/Release/output"
docker run -it --rm -p 5000:80 BlazorServerApp