FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DesafioBackend.sln ./
COPY DesafioBackend/DesafioBackend.csproj DesafioBackend/
COPY DesafioBackend.Tests/DesafioBackend.Tests.csproj DesafioBackend.Tests/ 

RUN dotnet restore

COPY . ./

WORKDIR /src
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT [ "dotnet", "DesafioBackend.dll" ]