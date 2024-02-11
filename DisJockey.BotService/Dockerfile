#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DisJockey.BotService/DisJockey.BotService.csproj", "DisJockey.BotService/"]
RUN dotnet restore "./DisJockey.BotService/./DisJockey.BotService.csproj"
COPY . .
WORKDIR "/src/DisJockey.BotService"
RUN dotnet build "./DisJockey.BotService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DisJockey.BotService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DisJockey.BotService.dll"]