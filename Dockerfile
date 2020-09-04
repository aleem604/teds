FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["VirtualEvents.API/VirtualEvents.API.csproj", "VirtualEvents.API/"]
COPY ["Common/S27.Gamification/S27.Gamification.csproj", "Common/S27.Gamification/"]
COPY ["Common/S27.Shared.Repositories/Shared.Repositories.csproj", "Common/S27.Shared.Repositories/"]
COPY ["Common/S27.Shared.Models/Shared.Models.csproj", "Common/S27.Shared.Models/"]
COPY ["VirtualEvents.Models/VirtualEvents.Models.csproj", "VirtualEvents.Models/"]
COPY ["Common/S27.DistributedCaching/S27.DistributedCaching.csproj", "Common/S27.DistributedCaching/"]
COPY ["Common/S27.Instrumentation/S27.Instrumentation.csproj", "Common/S27.Instrumentation/"]
COPY ["Common/S27.Configuration/S27.Configuration.csproj", "Common/S27.Configuration/"]
COPY ["S27.Infra.IoC/S27.Infra.IoC.csproj", "S27.Infra.IoC/"]
COPY ["VirtualEvents.Repositories/VirtualEvents.Repositories.csproj", "VirtualEvents.Repositories/"]
COPY ["Shared.Common/Shared.Common.csproj", "Shared.Common/"]
COPY ["VirtualEvents.Services/VirtualEvents.Services.csproj", "VirtualEvents.Services/"]
COPY ["S27.Core/S27.Core.csproj", "S27.Core/"]
COPY ["S27.Gamification.Shared/S27.Gamification.Shared.csproj", "S27.Gamification.Shared/"]
COPY ["S27.Rbac.Shared/S27.Rbac.Shared.csproj", "S27.Rbac.Shared/"]
COPY ["Common/S27.Config/S27.Config.csproj", "Common/S27.Config/"]
COPY ["Common/S27.RBACMiddleware/S27.Rbac.csproj", "Common/S27.RBACMiddleware/"]
COPY ["Common/S27.Auth/S27.Auth.csproj", "Common/S27.Auth/"]
RUN dotnet restore "VirtualEvents.API/VirtualEvents.API.csproj"
COPY . .
WORKDIR "/src/VirtualEvents.API"
RUN dotnet build "VirtualEvents.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VirtualEvents.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://+:5000
ENTRYPOINT ["dotnet", "VirtualEvents.API.dll"]
CMD ["tail", "-f", "/dev/null"]
