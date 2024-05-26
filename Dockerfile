# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY FiksComService/*.csproj ./FiksComService/
COPY FiksComServiceTests/*.csproj ./FiksComServiceTests/
RUN dotnet restore

# install dependencies for QuestPDF
#RUN dotnet add package SkiaSharp.NativeAssets.Linux.NoDependencies --version 2.88.8
#RUN dotnet add package HarfBuzzSharp.NativeAssets.Linux --version 8.3.0-preview.3.1

# copy everything else and build app
COPY FiksComService/. ./FiksComService/
COPY FiksComServiceTests/. ./FiksComServiceTests/
WORKDIR /source/FiksComService
RUN dotnet add package SkiaSharp.NativeAssets.Linux.NoDependencies --version 2.88.8
RUN dotnet add package HarfBuzzSharp.NativeAssets.Linux --version 8.3.0-preview.3.1
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "FiksComService.dll"]
