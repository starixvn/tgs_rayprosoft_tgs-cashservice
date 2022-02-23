#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.CashServiceWorker/Tgs.CashServiceWorker.csproj", "Tgs.CashServiceWorker/"]
COPY ["Tgs.CashServiceWorker.Core/Tgs.CashServiceWorker.Core.csproj", "Tgs.CashServiceWorker.Core/"]
COPY ["Tgs.CashService.Common/Tgs.CashService.Common.csproj", "Tgs.CashService.Common/"]
COPY ["Tgs.CashService.Repositories/Tgs.CashService.Repositories.csproj", "Tgs.CashService.Repositories/"]
RUN dotnet restore "Tgs.CashServiceWorker/Tgs.CashServiceWorker.csproj"
COPY . .
WORKDIR "/src/Tgs.CashServiceWorker"
RUN dotnet build "Tgs.CashServiceWorker.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.CashServiceWorker.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.CashServiceWorker.dll"]