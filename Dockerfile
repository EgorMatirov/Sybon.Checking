# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Sybon.Checking/*.csproj ./Sybon.Checking/
COPY libs ./libs
RUN dotnet restore

# copy everything else and build app
COPY Sybon.Checking/. ./Sybon.Checking/
WORKDIR /source/Sybon.Checking
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:2.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Sybon.Checking.dll"]