#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashServiceWorker/Tgs.CashServiceWorker.csproj", "Tgs.CashServiceWorker/"]
COPY ["Tgs.CashServiceWorker.Core/Tgs.CashServiceWorker.Core.csproj", "Tgs.CashServiceWorker.Core/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
COPY ["Tgs.CashServiceWorker.Repositories/Tgs.CashServiceWorker.Repositories.csproj", "Tgs.CashServiceWorker.Repositories/"]
RUN dotnet restore "./Tgs.CashServiceWorker/./Tgs.CashServiceWorker.csproj"
COPY . .
WORKDIR "/src/Tgs.CashServiceWorker"
RUN dotnet build "./Tgs.CashServiceWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.CashServiceWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashServiceWorker.dll"]