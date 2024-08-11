#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM node:16-alpine AS angular
WORKDIR /client
COPY ["DisJockey/ClientApp", ""]
RUN npm install
RUN npm run build-prod

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DisJockey/DisJockey.csproj", "DisJockey/"]
COPY ["DisJockey.Core/DisJockey.Core.csproj", "DisJockey.Core/"]
COPY ["DisJockey.MassTransit/DisJockey.MassTransit.csproj", "DisJockey.MassTransit/"]
COPY ["DisJockey.Application/DisJockey.Application.csproj", "DisJockey.Application/"]
COPY ["DisJockey.Infrastructure/DisJockey.Infrastructure.csproj", "DisJockey.Infrastructure/"]
COPY ["DisJockey.Shared/DisJockey.Shared.csproj", "DisJockey.Shared/"]
RUN dotnet restore "./DisJockey/DisJockey.csproj"
COPY . .
WORKDIR "/src/DisJockey"
RUN dotnet build "./DisJockey.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DisJockey.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=angular /wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "DisJockey.dll"]