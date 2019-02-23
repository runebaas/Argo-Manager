FROM microsoft/dotnet:2.2.1-aspnetcore-runtime

COPY . /app/

EXPOSE 5000/tcp 5001/tcp
WORKDIR /app
ENTRYPOINT /usr/bin/dotnet ArgoManager.dll
