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
COPY ["Tgs.CashServiceCommitter/Tgs.CashServiceCommitter.csproj", "Tgs.CashServiceCommitter/"]
COPY ["Tgs.CashServiceCommitter.Core/Tgs.CashServiceCommitter.Core.csproj", "Tgs.CashServiceCommitter.Core/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
COPY ["Tgs.CashServiceCommitter.Repositories/Tgs.CashServiceCommitter.Repositories.csproj", "Tgs.CashServiceCommitter.Repositories/"]
RUN dotnet restore "./Tgs.CashServiceCommitter/./Tgs.CashServiceCommitter.csproj"
COPY . .
WORKDIR "/src/Tgs.CashServiceCommitter"
RUN dotnet build "./Tgs.CashServiceCommitter.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.CashServiceCommitter.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashServiceCommitter.dll"]