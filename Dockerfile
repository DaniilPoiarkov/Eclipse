#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# COPY ["src/Eclipse.WebAPI/Eclipse.WebAPI.csproj", "."]
# COPY ["src/Eclipse.Application/Eclipse.Application.csproj", "."]
# COPY ["src/Eclipse.Application.Contracts/Eclipse.Application.Contracts.csproj", "."]
# COPY ["src/Eclipse.Core/Eclipse.Core.csproj", "."]
# COPY ["src/Eclipse.DataAccess/Eclipse.DataAccess.csproj", "."]
# COPY ["src/Eclipse.Domain/Eclipse.Domain.csproj", "."]
# COPY ["src/Eclipse.Domain.Shared/Eclipse.Domain.Shared.csproj", "."]
# COPY ["src/Eclipse.Infrastructure/Eclipse.Infrastructure.csproj", "."]
# COPY ["src/Eclipse.Localization/Eclipse.Localization.csproj", "."]
# COPY ["src/Eclipse.Pipelines/Eclipse.Pipelines.csproj", "."]
# COPY ["Common/Eclipse.Common/Eclipse.Common.csproj", "."]
COPY . .

RUN dotnet restore "src/Eclipse.WebAPI/Eclipse.WebAPI.csproj"
COPY . .

RUN dotnet build "src/Eclipse.WebAPI/Eclipse.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Eclipse.WebAPI/Eclipse.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eclipse.WebAPI.dll"]
