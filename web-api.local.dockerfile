#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Install
# curl for cosmos cert retrieval
# fonts for reports 
# jq for webhook configuration
RUN apt-get update && \
    apt-get install -y curl && \ 
    apt-get install -y fontconfig && \
    apt-get install -y jq && \
    apt-get install -y dos2unix && \
    apt-get install -y ttf-mscorefonts-installer

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY /src .
COPY Directory.Build.props .
COPY Directory.Packages.props .

RUN dotnet restore "Eclipse.WebAPI/Eclipse.WebAPI.csproj"
COPY /src .

RUN dotnet build "Eclipse.WebAPI/Eclipse.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Eclipse.WebAPI/Eclipse.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./scripts .

RUN find *.sh -type f | xargs dos2unix

ENTRYPOINT ["./entrypoint.sh", "cosmos.domain", "8081"]
