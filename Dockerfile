FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet build ./WebService/WebService.csproj -c Release


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/WebService/bin/Release/net6.0 .
EXPOSE 80
ENTRYPOINT dotnet WebService.dll environment=uat

