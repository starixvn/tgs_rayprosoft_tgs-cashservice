#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["RefreshAllWalletsTool/RefreshAllWalletsTool.csproj", "RefreshAllWalletsTool/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
RUN dotnet restore "./RefreshAllWalletsTool/./RefreshAllWalletsTool.csproj"
COPY . .
WORKDIR "/src/RefreshAllWalletsTool"
RUN dotnet build "./RefreshAllWalletsTool.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RefreshAllWalletsTool.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RefreshAllWalletsTool.dll"]